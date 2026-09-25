// Smoke tests for the npm port. Run after build: node test/smoke.test.mjs
import assert from "node:assert";
import {
  ready,
  convertToNepaliFromString,
  convertToAd,
  getMonthDateInAd,
  getYearDateRangeInAd,
  getAvailableCalendarYearsBs,
  getCurrentDateInfo,
  getDayDetails,
  getEvents,
  getEventsForDate,
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

// Rich events and daily details are separate data sets.
const years = getAvailableCalendarYearsBs();
console.log(`Supported BS years: ${years.minYear}-${years.maxYear}`);
const events = await getEvents(2081);
assert.ok(Array.isArray(events));
console.log(`Events for 2081 BS: ${events.length}`);

const sample = await getDayDetails(new NepaliDate(2083, 6, 5));
assert.strictEqual(sample.adDate, "2026-09-21");
assert.strictEqual(sample.tithi, 10);
assert.strictEqual(sample.chandrama, 21);
assert.strictEqual(sample.nsYear, 1146);
assert.strictEqual(sample.paksha, "शुक्ल");
assert.strictEqual(sample.bsDate.toString(), "2083-06-05");
const eventsForDate = await getEventsForDate(new NepaliDate(2081, 7, 17));
assert.ok(Array.isArray(eventsForDate));

const info = await getCurrentDateInfo();
console.log("Current date:", info.date.toString(), `(${info.monthName})`, "AD:", info.adDate);

console.log("\nAll smoke tests passed ✔");
