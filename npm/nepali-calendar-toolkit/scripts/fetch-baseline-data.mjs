// Downloads a baseline snapshot of the calendar data from the NepaliCalendarToolkit
// repository into src/data so the package always works offline (mirrors the C#
// package's embedded resources). Run automatically before publishing.
import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const BASE_URL =
  process.env.DATA_URL || "https://cdn.jsdelivr.net/gh/shoesheill/NepaliCalendarToolkit@main/";
const OUT_DIR = join(dirname(fileURLToPath(import.meta.url)), "..", "src", "data");

async function download(path) {
  const res = await fetch(new URL(path, BASE_URL));
  if (!res.ok) throw new Error(`Failed to fetch ${path}: HTTP ${res.status}`);
  // Data repo files may contain `//` comments; strip them for strict JSON.
  const clean = (t) => t.replace(/\/\/.*$/gm, "").replace(/,\s*([}\]])/gm, "$1");
  const json = JSON.parse(clean(await res.text()));
  const out = join(OUT_DIR, path);
  mkdirSync(dirname(out), { recursive: true });
  writeFileSync(out, JSON.stringify(json));
  console.log(`saved ${path} (${Buffer.byteLength(JSON.stringify(json))} bytes)`);
}

const years = process.argv[2] ? process.argv[2].split(",").map(Number) : [];
await download("month-lengths.json");
await download("year-start.json");

const mlText = await (await fetch(new URL("month-lengths.json", BASE_URL))).text();
const monthLengths = JSON.parse(mlText.replace(/\/\/.*$/gm, "").replace(/,\s*([}\]])/gm, "$1"));
const allYears = Object.keys(monthLengths).map(Number).sort((a, b) => a - b);
const yearsToFetch = years.length ? years : allYears.filter((y) => y >= allYears[allYears.length - 1] - 15);
for (const year of yearsToFetch) await download(`Holidays/${year}.json`);
