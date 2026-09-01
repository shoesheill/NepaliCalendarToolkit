// Smoke tests for the npm port. Run after build: node test/smoke.test.mjs
import assert from "node:assert";
import {
  ready,
  convertToNepaliFromString,
  convertToAd,
  getMonthDateInAd,
  getYearDateRangeInAd,
  getHolidaysAndWeekends,
  getAvailableCalendarYearsBs,
  getCurrentDateInfo,
  configureWeekendDays,
  isWeekend,
} from "../dist/index.js";

const { NepaliDate } = await import("../dist/index.js");

await ready;

// Known pair: 2075-01-01 BS === 2018-04-14 AD
const bs = await convertToNepaliFromString("2018-04-14");
assert.strictEqual(bs.toString(), "2075-01-01", `expected 2075-01-01, got ${bs}`);
const ad = await convertToAd(new NepaliDate(2075, 1, 1));
assert.strictEqual(ad, "2018-04-14", `expected 2018-04-14, got ${ad}`);

// Round-trip a mid-year date: 2081-06-15
const mid = await convertToNepaliFromString(await convertToAd(new NepaliDate(2081, 6, 15)));
assert.strictEqual(mid.toString(), "2081-06-15");

// Month range
const monthRange = await getMonthDateInAd(2075, 1);
assert.strictEqual(monthRange.startDate, "2018-04-14");
assert.strictEqual(monthRange.endDate, "2018-05-14", `got ${monthRange.endDate}`); // Baisakh 2075 has 31 days

// Year range
const yearRange = await getYearDateRangeInAd(2075);
assert.strictEqual(yearRange.startDate, "2018-04-14");
assert.ok(yearRange.endDate > yearRange.startDate);

// Weekend config (default Sat+Sun; JS 6=Sat, 0=Sun)
assert.ok(isWeekend(6));
configureWeekendDays(6); // Saturday only
assert.ok(isWeekend(6) && !isWeekend(0));
configureWeekendDays(0, 6);

// Holidays for a recent year (needs holiday data in baseline)
const years = getAvailableCalendarYearsBs();
console.log(`Supported BS years: ${years.minYear}-${years.maxYear}`);
const holidays = await getHolidaysAndWeekends(2081, undefined, "holidays");
assert.ok(Array.isArray(holidays));
console.log(`Holidays for 2081 BS: ${holidays.length}`);

const info = await getCurrentDateInfo();
console.log("Current date:", info.date.toString(), `(${info.monthName})`, "AD:", info.adDate);

console.log("\nAll smoke tests passed ✔");
