using System;
using System.Collections.Generic;
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
            var payload = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["date"] = $"{bsYear:D4}-{bsMonth:D2}-{bsDay:D2}",
                ["based_on"] = "BS"
            });

            return await WithRetryAsync(async () =>
            {
                using var response = await _http.PostAsync($"{_baseUrl}/calendars/dateConvert", payload);
                var text = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(
                        $"dateConvert failed ({response.StatusCode}) for {bsYear:D4}-{bsMonth:D2}-{bsDay:D2}: {text}");

                return JsonSerializer.Deserialize<DateConvertResponse>(text, JsonOptions());
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
                var text = await _http.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(text) || text.TrimStart().StartsWith("{"))
                    return null; // JSON object (error envelope) -> not available yet

                return JsonSerializer.Deserialize<List<GovernmentHolidayResponse>>(text, JsonOptions());
            });
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
                || (ex is HttpRequestException h && h.InnerException is SocketException);
        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
    }
}
