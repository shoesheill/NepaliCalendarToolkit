import { HolidayData } from "./types";

/**
 * Tiered data provider mirroring the C# DataProvider: live CDN → in-memory cache →
 * bundled baseline snapshot. The baseline JSON files are downloaded at build time
 * by scripts/fetch-baseline-data.mjs from the same Nepali-Calendar-Data repository.
 */

let baseUrl =
  process.env.DATA_URL || "https://cdn.jsdelivr.net/gh/shoesheill/Nepali-Calendar-Data@master/";
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
  const res = await fetch(new URL(path, baseUrl), {
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

  const baseline = normalizeJson<T>(require(`./data/${path}`));
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

export async function getHolidays(year: number): Promise<HolidayData[]> {
  const data = await getData<HolidayData[]>(`Holidays/${year}.json`);
  return Array.isArray(data) ? data : [];
}
