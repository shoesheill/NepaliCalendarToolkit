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

## Datepicker primitives

The BS month-grid view model used by datepickers is part of the public API, so a
host can render a calendar in any framework (or plain DOM) without re-deriving
weekdays, holidays or month lengths:

```js
import { buildMonthGrid, daysInBsMonth, monthNames, formatBsLong, sameBsDay } from "nepali-calendar-toolkit";

await ready;
const grid = await buildMonthGrid(2083, 6, "2083-06-04"); // year, month, todayKey?
grid.leadingBlanks;        // 0=Sun..6=Sat padding before day 1
grid.cells[0];             // { day, bsKey, adDate, weekday, weekend, holidayName?, today }
```

## React date picker (`nepali-calendar-toolkit/react`)

A ready-made React popup built on the grid primitives above. It lives at the
`/react` subpath so `import "nepali-calendar-toolkit"` stays React-free — `react`
is an **optional** peer dependency, and the core entry point still runs in plain
Node with zero runtime dependencies.

```bash
npm install nepali-calendar-toolkit react    # react only needed for the /react entry
```

```tsx
import { BsDatePicker } from "nepali-calendar-toolkit/react";
import "nepali-calendar-toolkit/styles.css"; // optional plain-CSS defaults

<BsDatePicker
  value={value}                 // "2082-04-15" (BS, yyyy-MM-dd)
  onChange={(s) => setValue(s?.bs ?? null)}
  min="2082-04-01"
  max="2082-04-32"
  locale="ne"                   // Devanagari month/weekday labels + digits
  classNames={{ daySelected: "bg-purple-600 text-white" }}
/>
```

Bring your own trigger and render only the popup:

```tsx
import { BsDatePickerPopup } from "nepali-calendar-toolkit/react";
```

The `onChange` payload is `{ bs, value: { year, month, day }, ad, holidayName?, weekend }`.
Exports: `BsDatePicker`, `BsDatePickerPopup`, `BsDateTrigger` and the headless
`createBsPickerController` / `isBsKeyDisabled` helpers, so a Vue/Angular/Svelte
wrapper can reuse the same state machine. Without `styles.css` the `nct-*` classes
remain for a host stylesheet to override.

## Data strategy (mirrors the C# package)

1. **Bundled baseline** — a snapshot of `Data/month-lengths.json`, `Data/year-start.json` and `Data/Holidays/*.json` copied into `src/data` at build time by `scripts/fetch-baseline-data.mjs` (from the [NepaliCalendarToolkit](https://github.com/shoesheill/NepaliCalendarToolkit) repo). Guarantees the package always works offline.
2. **Live CDN** — at runtime the same repository's `Data/` directory is fetched (default `https://cdn.jsdelivr.net/gh/shoesheill/NepaliCalendarToolkit@main/`, overridable via the `DATA_URL` env var or `configure(url, ttlHours)`) and merged over the baseline, so newly seeded years appear without a package release.

Timezone handling uses `Intl` (`Asia/Kathmandu`) — no OS timezone database dependency, works in Node and browsers.

## Development

```bash
node scripts/fetch-baseline-data.mjs   # refresh bundled baseline data
npm test                               # build + core, browser and /react tests
npm publish                            # prepublishOnly builds and re-runs the same tests
```

`react`, `react-dom` and `jsdom` are **dev-only** here: the core entry point must
stay React-free, so `src/react/` is compiled by its own `tsconfig.react.json` into
`dist/react/`. A test asserts that `dist/index.js` never requires `react`.
