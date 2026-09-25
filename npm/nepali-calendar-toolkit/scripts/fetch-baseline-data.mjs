// Downloads a baseline snapshot of the calendar data from the NepaliCalendarToolkit
// repository into src/data so the package always works offline (mirrors the C#
// package's embedded resources). Run automatically before publishing.
import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

// The CDN mirrors the repository root and data lives under Data/, so a custom DATA_URL
// must point at a repository root that contains Data/month-lengths.json (a trailing
// slash is added when missing).
const BASE_URL =
  (process.env.DATA_URL || "https://cdn.nepali.calendar.localhub.dev/").replace(/\/?$/, "/");
const DATA_ROOT = "Data/";
const OUT_DIR = join(dirname(fileURLToPath(import.meta.url)), "..", "src", "data");

async function download(path, { optional = false } = {}) {
  const sourcePath = `${DATA_ROOT}${path}`;
  const res = await fetch(new URL(sourcePath, BASE_URL));
  if (!res.ok) {
    if (optional && res.status === 404) {
      console.warn(`skipped ${path} (not published)`);
      return undefined;
    }
    throw new Error(`Failed to fetch ${sourcePath}: HTTP ${res.status}`);
  }
  // Data repo files may contain `//` comments; strip them for strict JSON.
  const clean = (t) => t.replace(/\/\/.*$/gm, "").replace(/,\s*([}\]])/gm, "$1");
  const json = JSON.parse(clean(await res.text()));
  const out = join(OUT_DIR, path);
  mkdirSync(dirname(out), { recursive: true });
  writeFileSync(out, JSON.stringify(json));
  console.log(`saved ${path} (${Buffer.byteLength(JSON.stringify(json))} bytes)`);
  return json;
}

const years = process.argv[2] ? process.argv[2].split(",").map(Number) : [];
// Keep the npm offline baseline aligned with the 20-year event/day-detail snapshot
// (currently 2065-2084). Core calendar metadata is always downloaded above.
const BUNDLED_YEAR_COUNT = 20;
const monthLengths = await download("month-lengths.json");
await download("year-start.json");
// Provenance per year (knownMonths / provisional flags), read by the core on load.
await download("month-meta.json");
const allYears = Object.keys(monthLengths).map(Number).sort((a, b) => a - b);
const yearsToFetch = years.length
  ? years
  : allYears.filter((y) => y >= allYears[allYears.length - 1] - (BUNDLED_YEAR_COUNT - 1));
for (const year of yearsToFetch) {
  await download(`Events/${year}.json`, { optional: true });
  await download(`DayDetails/${year}.json`, { optional: true });
}
