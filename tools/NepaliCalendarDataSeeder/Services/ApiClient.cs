using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NepaliCalendarDataSeeder.Models;

namespace NepaliCalendarDataSeeder.Services
{
    /// <summary>
    ///     Thin HTTP wrapper around the Nepal Patro API. The dateConvert endpoint only
    ///     accepts application/x-www-form-urlencoded payloads (sending JSON returns 422).
    /// </summary>
    public class ApiClient
    {
        private const int MaxAttempts = 3;

        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly TimeSpan _minimumRequestInterval;
        private DateTime _lastRequestUtc;

        public string BaseUrlForLogging => _baseUrl;

        public ApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');

            // The API is fronted by Cloudflare and publishes both A (IPv4) and AAAA
            // (IPv6) records. In networks without IPv6 routing the first new connection
            // stalls on the unreachable IPv6 address and only falls back to IPv4 after a
            // long delay (observed 10-60s+), which regularly trips a 60s HttpClient
            // timeout on the very first request of a run and aborts the whole seed.
            // Establish connections over IPv4 first (falling back to any address when no
            // A record exists) so the first request comes back in well under a second.
            var handler = new SocketsHttpHandler
            {
                ConnectCallback = ConnectOverIpv4Async,
                ConnectTimeout = TimeSpan.FromSeconds(15)
            };

            // Generous overall timeout; per-operation retries below absorb edge cases.
            _http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(100) };
            _http.DefaultRequestHeaders.Add("User-Agent", "NepaliCalendarDataSeeder/1.0");
            _http.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");

            var delayMs = 100;
            if (int.TryParse(Environment.GetEnvironmentVariable("API_MIN_DELAY_MS"), out var configuredDelayMs))
                delayMs = Math.Max(0, configuredDelayMs);
            _minimumRequestInterval = TimeSpan.FromMilliseconds(delayMs);
            _lastRequestUtc = DateTime.MinValue;
        }

        /// <summary>
        ///     Resolves the target host and connects to an IPv4 address when one exists,
        ///     sidestepping the slow/broken IPv6 path described above.
        /// </summary>
        private static async ValueTask<Stream> ConnectOverIpv4Async(
            SocketsHttpConnectionContext context, CancellationToken cancellationToken)
        {
            var addresses = await Dns.GetHostAddressesAsync(context.DnsEndPoint.Host, cancellationToken);
            var target = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                      ?? addresses[0];

            var socket = new Socket(target.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            try
            {
                await socket.ConnectAsync(target, context.DnsEndPoint.Port, cancellationToken);
                return new NetworkStream(socket, ownsSocket: true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        /// <summary>
        ///     Converts a BS date to AD (and returns NS fields) via the dateConvert endpoint.
        /// </summary>
        public async Task<DateConvertResponse> ConvertBsAsync(int bsYear, int bsMonth, int bsDay)
        {
            return await WithRetryAsync(async () =>
            {
                await WaitBeforeRequestAsync();
                using var payload = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["date"] = $"{bsYear:D4}-{bsMonth:D2}-{bsDay:D2}",
                    ["based_on"] = "BS"
                });
                using var response = await _http.PostAsync($"{_baseUrl}/calendars/dateConvert", payload);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(
                        $"dateConvert failed ({response.StatusCode}) for {bsYear:D4}-{bsMonth:D2}-{bsDay:D2}: {text}",
                        null,
                        response.StatusCode);

                var result = JsonSerializer.Deserialize<DateConvertResponse>(text, JsonOptions());
                ValidateDateConvert(result, bsYear, bsMonth, bsDay);
                return result;
            });
        }

        /// <summary>
        ///     Fetches government holidays for a BS year. Returns null when the API reports
        ///     an error or empty list (i.e. data not yet available for that year).
        /// </summary>
        public async Task<List<GovernmentHolidayResponse>> GetGovernmentHolidaysAsync(int bsYear)
        {
            var url = $"{_baseUrl}/goverment-holidays/{bsYear}";
            return await WithRetryAsync(async () =>
            {
                await WaitBeforeRequestAsync();
                using var response = await _http.GetAsync(url);
                var text = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(
                        $"government events failed ({response.StatusCode}) for BS {bsYear}: {text}",
                        null,
                        response.StatusCode);

                if (string.IsNullOrWhiteSpace(text) || text.TrimStart().StartsWith("{"))
                    return null; // JSON object (error envelope) -> not available yet

                var result = JsonSerializer.Deserialize<List<GovernmentHolidayResponse>>(text, JsonOptions());
                if (result == null) throw new JsonException($"Invalid event response for BS {bsYear}.");
                return result;
            });
        }

        private async Task WaitBeforeRequestAsync()
        {
            var elapsed = DateTime.UtcNow - _lastRequestUtc;
            var remaining = _minimumRequestInterval - elapsed;
            if (remaining > TimeSpan.Zero)
                await Task.Delay(remaining);
            _lastRequestUtc = DateTime.UtcNow;
        }

        private static void ValidateDateConvert(DateConvertResponse response, int year, int month, int day)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.Ad))
                throw new JsonException($"dateConvert returned no AD date for {year:D4}-{month:D2}-{day:D2}.");

            if (!DateTime.TryParseExact(response.Ad, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
                throw new JsonException($"dateConvert returned invalid AD date '{response.Ad}'.");

            if (response.BsYear.HasValue && response.BsYear.Value != year ||
                response.BsMonth.HasValue && response.BsMonth.Value != month ||
                response.BsDay.HasValue && response.BsDay.Value != day)
                throw new JsonException(
                    $"dateConvert returned mismatched BS date for {year:D4}-{month:D2}-{day:D2}: " +
                    $"{response.BsYear:D4}-{response.BsMonth:D2}-{response.BsDay:D2}.");
        }

        /// <summary>
        ///     Retries an HTTP operation a few times when the failure is transient (a
        ///     timeout or a dropped connection). A single slow/stalled request must not
        ///     abort the seeding of an entire year.
        /// </summary>
        private static async Task<T> WithRetryAsync<T>(Func<Task<T>> operation)
        {
            Exception last = null;
            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    last = ex;
                    if (attempt == MaxAttempts) break;
                    await Task.Delay(TimeSpan.FromMilliseconds(750 * attempt));
                }
            }

            last ??= new Exception("Request failed");
            throw new Exception($"Request failed after {MaxAttempts} attempts.", last);
        }

        private static bool IsTransient(Exception ex)
        {
            return ex is TaskCanceledException or OperationCanceledException
                || (ex is HttpRequestException h &&
                    (h.InnerException is SocketException ||
                     h.StatusCode == HttpStatusCode.TooManyRequests ||
                     (h.StatusCode.HasValue && (int)h.StatusCode.Value >= 500)));
        }

        private static JsonSerializerOptions JsonOptions()
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // The dateConvert / holiday endpoints sometimes return optional numeric
            // fields as blank or non-numeric strings for unverified future dates.
            options.Converters.Add(new LenientNullableIntConverter());
            return options;
        }
    }
}
