# nepali-calendar-toolkit (npm)

TypeScript/JavaScript port of the C# [NepaliCalendarToolkit](https://github.com/shoesheill/NepaliCalendarToolkit) NuGet package — Bikram Sambat ⇄ AD conversion, configurable weekends, holidays, and date ranges for months, weeks, quarters, fiscal years, or any custom period.

## Install

```bash
npm install nepali-calendar-toolkit
```

## Usage

```js
import {
  ready, convertToNepali, convertToAd, NepaliDate,
  getMonthDateInAd, getYearDateRangeInAd, getQuarterDateRangeInAd,
  getHolidaysAndWeekends, getCurrentDateInfo, configureWeekendDays,
} from "nepali-calendar-toolkit";

await ready; // loads baseline data + CDN refresh

// Conversion
const bs = await convertToNepali(new Date());          // NepaliDate { year, month, day }
const ad = await convertToAd(new NepaliDate(2081, 1, 1)); // "2024-04-13"

// Ranges (all return { startDate, endDate } as yyyy-MM-dd AD strings)
const month = await getMonthDateInAd(2081, 5);         // one BS month
const quarter = await getQuarterDateRangeInAd(2081, 1);
const year = await getYearDateRangeInAd(2081, true);   // fiscal year (Shrawan → Ashar)

// Holidays & weekends
configureWeekendDays(6); // JS day numbers: 0=Sun ... 6=Sat
const holidays = await getHolidaysAndWeekends(2081, undefined, "holidays");

// Today's info
const info = await getCurrentDateInfo(true); // includes monthName, quarter, fiscalYear, isWeekend...
```

## Data strategy (mirrors the C# package)

1. **Bundled baseline** — a snapshot of `month-lengths.json`, `year-start.json` and `Holidays/*.json` copied into `src/data` at build time by `scripts/fetch-baseline-data.mjs` (from the [Nepali-Calendar-Data](https://github.com/shoesheill/Nepali-Calendar-Data) repo). Guarantees the package always works offline.
2. **Live CDN** — at runtime the same repository is fetched (default `https://cdn.jsdelivr.net/gh/shoesheill/Nepali-Calendar-Data@master/`, overridable via the `DATA_URL` env var or `configure(url, ttlHours)`) and merged over the baseline, so newly seeded years appear without a package release.

Timezone handling uses `Intl` (`Asia/Kathmandu`) — no OS timezone database dependency, works in Node and browsers.

## Development

```bash
node scripts/fetch-baseline-data.mjs   # refresh bundled baseline data
npm test                               # build + smoke tests
npm publish                            # prepublishOnly builds and re-runs smoke tests
```
