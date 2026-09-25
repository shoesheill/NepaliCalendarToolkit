import { CalendarEvent, DayDetailsData } from "./types";
import { BASELINE } from "./baselineData";

/**
 * Tiered data provider mirroring the C# DataProvider: live CDN → in-memory cache →
 * bundled baseline snapshot. The baseline JSON files are downloaded at build time
 * by scripts/fetch-baseline-data.mjs from the same CDN host.
 *
 * The CDN mirrors the repository root and calendar data lives under `Data/`, so every
 * CDN-relative path getData() asks for ("Events/2083.json", "month-lengths.json") is
 * prefixed with DATA_ROOT.
 */

/**
 * `process` only exists in Node. Reading it unguarded threw ReferenceError the moment a
 * browser imported this module, taking the whole library down before `configure()` could
 * ever run — so every web consumer silently lost all conversions.
 */
function envDataUrl(): string | undefined {
  const value = typeof process !== "undefined" && process.env ? process.env.DATA_URL : undefined;
  if (!value) return undefined;
  // Without a trailing slash a relative path would silently replace the base's last
  // segment, so normalise once here instead of relying on the caller.
  return value.endsWith("/") ? value : value + "/";
}

let baseUrl = envDataUrl() || "https://cdn.nepali.calendar.localhub.dev/";
const DATA_ROOT = "Data/";
export let cacheTtlHours = 12;

const memoryCache = new Map<string, { data: unknown; fetchedAt: number }>();

export function configure(baseUrlOption?: string, cacheTtlHoursOption?: number): void {
  if (baseUrlOption) baseUrl = baseUrlOption.endsWith("/") ? baseUrlOption : baseUrlOption + "/";
  if (cacheTtlHoursOption && cacheTtlHoursOption > 0) cacheTtlHours = cacheTtlHoursOption;
}

function normalizeJson<T>(raw: unknown): T | undefined {
  return raw && typeof raw === "object" ? (raw as T) : undefined;
}

/** The data repo files contain `//` comments; strip them before parsing. */
function stripJsonComments(text: string): string {
  return text.replace(/\/\/.*$/gm, "").replace(/,\s*([}\]])/gm, "$1");
}

async function fetchJson(path: string): Promise<unknown | undefined> {
  const res = await fetch(new URL(`${DATA_ROOT}${path}`, baseUrl), {
    headers: { "User-Agent": "nepali-calendar-toolkit/1.0" },
    signal: AbortSignal.timeout(15_000),
  });
  if (!res.ok) return undefined;
  return JSON.parse(stripJsonComments(await res.text()));
}

/**
 * Resolves data for a CDN-relative path. For JSON objects keyed by BS year
 * (month-lengths.json, year-start.json) a CDN refresh is merged over the bundled
 * baseline so newly added years appear without a package release.
 */
export async function getData<T>(path: string): Promise<T | undefined> {
  const cached = memoryCache.get(path);
  if (cached && Date.now() - cached.fetchedAt < cacheTtlHours * 3600 * 1000) {
    return cached.data as T;
  }

  const baseline = normalizeJson<T>(BASELINE[path]);
  let result: T | undefined = baseline;

  try {
    const remote = normalizeJson(await fetchJson(path));
    if (remote) {
      // Merge so remote additions win, baseline fills gaps (per-year keys or arrays).
      if (typeof remote === "object" && !Array.isArray(remote)) {
        result = { ...(baseline as object), ...(remote as object) } as T;
      } else {
        result = remote as T;
      }
    }
  } catch {
    // Offline / blocked network: baseline already set, keep going.
  }

  memoryCache.set(path, { data: result, fetchedAt: Date.now() });
  return result;
}

export async function getEvents(year: number): Promise<CalendarEvent[]> {
  const data = await getData<CalendarEvent[]>(`Events/${year}.json`);
  return Array.isArray(data) ? data : [];
}

export async function getDayDetails(year: number): Promise<DayDetailsData[]> {
  const data = await getData<DayDetailsData[]>(`DayDetails/${year}.json`);
  return Array.isArray(data) ? data : [];
}
