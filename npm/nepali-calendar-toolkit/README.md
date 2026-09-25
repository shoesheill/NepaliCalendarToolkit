# Nepali Calendar Toolkit for JavaScript and TypeScript

A practical Nepali (Bikram Sambat, BS) calendar toolkit for applications, dashboards, reports, and business workflows. Convert dates, generate reporting periods, work with holidays and weekends, and add a customizable Nepali date picker to web applications.

![Nepali Calendar Toolkit for JavaScript and React overview](https://cdn.jsdelivr.net/gh/shoesheill/NepaliCalendarToolkit@main/npm/nepali-calendar-toolkit/assets/nepali-calendar-toolkit-overview.svg)

## Features

- **Get AD date ranges for Nepali months, weeks, month sequences, quarters, calendar years, and fiscal years—without manually handling variable month lengths or fiscal-year boundaries**
- Add a ready-to-use React date picker with English and Nepali labels; build custom picker UIs in other frameworks with the exported calendar-grid primitives
- Customize date-picker colors, selected days, holidays, weekends, disabled dates, typography, and layout
- Convert Gregorian (AD) dates to Nepali (BS) dates and back
- Retrieve public holidays and configured weekends by year, month, or fiscal year
- Filter results to holidays, weekends, or both
- Configure weekend days; Saturday and Sunday are the defaults
- Get current-date information, including BS date, quarter, week of month, fiscal year, weekday, and weekend status
- Build a Nepali month grid in plain JavaScript or any UI framework
- Check supported years, available months, complete years, and provisional dates
- Use the core package in Node.js and browsers without requiring React

## Installation

```bash
npm install nepali-calendar-toolkit
```

The core package supports Node.js 18 and newer. React is optional and is only needed for the React date picker.

## Quick Start

Import the functions you need and await `ready` once before the first calendar operation:

```ts
import {
  ready,
  NepaliDate,
  convertToNepali,
  convertToAd,
  convertToNepaliFromString,
} from "nepali-calendar-toolkit";

await ready;

const bs = await convertToNepali(
  new Date("2024-04-13T00:00:00+05:45"),
);
console.log(bs.year, bs.month, bs.day); // 2081 1 1
console.log(bs.toString());             // 2081-01-01

const ad = await convertToAd(new NepaliDate(2081, 1, 1));
console.log(ad); // 2024-04-13

const fromString = await convertToNepaliFromString("2024-04-13");
console.log(fromString.toString()); // 2081-01-01
```

`convertToNepali` accepts a JavaScript `Date`. `convertToAd` returns an AD date in `yyyy-MM-dd` format.

## Get Nepali Months, Quarters, and Fiscal-Year Date Ranges

> **Generate AD start and end dates for Nepali reporting periods without manually calculating month lengths, quarter boundaries, or fiscal-year transitions.**

All range methods return `{ startDate, endDate }`, with both values in `yyyy-MM-dd` format.

```ts
import {
  getMonthDateInAd,
  getWeekDateInAd,
  getMonthRangeDateInAd,
  getQuarterDateRangeInAd,
  getYearDateRangeInAd,
} from "nepali-calendar-toolkit";

// One Nepali month
const month = await getMonthDateInAd(2081, 1);

// One week within a Nepali month
const week = await getWeekDateInAd(2081, 1, 2);

// A sequence of months
const months = await getMonthRangeDateInAd(2081, 6, 8);

// A calendar quarter
const quarter = await getQuarterDateRangeInAd(2081, 1);

// A fiscal-year quarter
const fiscalQuarter = await getQuarterDateRangeInAd(2080, 1, true);

// A complete BS calendar year
const calendarYear = await getYearDateRangeInAd(2081);

// A fiscal year: Shrawan 2080 through the end of Ashadh 2081
const fiscalYear = await getYearDateRangeInAd(2080, true);

console.log(fiscalYear.startDate, fiscalYear.endDate);
```

Pass `true` as the final argument when `yearBs` represents the starting year of a Nepali fiscal year. Fiscal quarters follow the Nepali fiscal calendar: Shrawan–Ashwin, Kartik–Poush, Magh–Chaitra, and Baisakh–Ashadh.

## Holidays and Weekends

```ts
import {
  configureWeekendDays,
  getConfiguredWeekendDays,
  getHolidaysAndWeekends,
} from "nepali-calendar-toolkit";

// JavaScript weekday numbers: 0 = Sunday ... 6 = Saturday
configureWeekendDays(6);    // Saturday only
configureWeekendDays(0, 6); // Sunday and Saturday
configureWeekendDays(5, 6); // Friday and Saturday

const weekendDays = getConfiguredWeekendDays();

// Holidays and weekends for a BS year
const yearDays = await getHolidaysAndWeekends(2081);

// Holidays for one month
const monthHolidays = await getHolidaysAndWeekends(
  2081,
  1,
  "holidays",
);

// Weekends for one month
const monthWeekends = await getHolidaysAndWeekends(
  2081,
  1,
  "weekends",
);

// Holidays and weekends for a full fiscal year
const fiscalYearDays = await getHolidaysAndWeekends(
  2080,
  undefined,
  "both",
  true,
);
```

Each result contains its weekday, holiday name, AD date, and BS date. The holiday name is empty for a weekend-only result.

## Current Date and Calendar Coverage

```ts
import {
  getCurrentDateInfo,
  getAvailableCalendarYearsBs,
  supportedYears,
  knownMonths,
  completeYears,
  isProvisional,
  isSupportedMonth,
  getYearInfo,
} from "nepali-calendar-toolkit";

const today = await getCurrentDateInfo();
const fiscalToday = await getCurrentDateInfo(true);

const range = getAvailableCalendarYearsBs();
const years = supportedYears();
const monthsKnown = knownMonths(2081);
const completed = completeYears();
const provisional = isProvisional(2081);
const shrawanAvailable = isSupportedMonth(2081, 4);
const yearDetails = getYearInfo(2081);

console.log(today.date.toString(), today.quarter, today.weekOfMonth);
console.log(today.monthName, today.dayOfWeek, today.isWeekend);
console.log(fiscalToday.quarter, fiscalToday.fiscalYear);
console.log(range.minYear, range.maxYear);
console.log(monthsKnown, completed.length, provisional);
console.log(shrawanAvailable, yearDetails);
```

## React Nepali Date Picker

Install React if it is not already part of your application:

```bash
npm install react nepali-calendar-toolkit
```

Import the picker from its dedicated entry point and include the optional default styles:

```tsx
import { useState } from "react";
import { BsDatePicker } from "nepali-calendar-toolkit/react";
import "nepali-calendar-toolkit/styles.css";

export function NepaliDateInput() {
  const [value, setValue] = useState<string | null>("2082-04-15");

  return (
    <BsDatePicker
      value={value}
      onChange={(selection) => setValue(selection?.bs ?? null)}
      min="2082-04-01"
      max="2082-04-32"
      initialMonth={[2082, 4]}
      disabledWeekdays={[6]}
      locale="ne"
      placeholder="Select Nepali date"
    />
  );
}
```

The picker provides:

- Month and year navigation
- English (`en`) and Nepali (`ne`) month and weekday labels
- Optional Devanagari digits
- Selected, today, holiday, weekend, and disabled-day states
- Minimum and maximum BS dates
- Individual disabled dates and disabled weekdays
- AD-date display for the selected day
- Controlled and externally managed popup behavior

The `onChange` payload includes the selected BS date, structured BS year/month/day, AD equivalent, optional holiday name, and weekend status.

## Customize the Date Picker

### CSS Variables

The default stylesheet can be themed with CSS variables:

```css
.nepali-date-picker {
  --nct-accent: #7c3aed;
  --nct-bg: #ffffff;
  --nct-fg: #1f2937;
  --nct-border: #d1d5db;
  --nct-hover: #f3e8ff;
  --nct-danger: #dc2626;
  --nct-muted: #6b7280;
}

.nepali-date-picker .nct-trigger {
  border-radius: 9999px;
  min-width: 15rem;
}

.nepali-date-picker .nct-popup {
  width: 21rem;
  border-radius: 1rem;
  box-shadow: 0 20px 45px rgb(0 0 0 / 0.18);
}
```

Wrap the component in the themed element:

```tsx
<div className="nepali-date-picker">
  <BsDatePicker {...props} />
</div>
```

### Custom Class Names

Use `classNames` to merge application or framework classes into specific picker parts:

```tsx
<BsDatePicker
  {...props}
  triggerClassName="my-date-trigger"
  classNames={{
    root: "my-picker-panel",
    header: "my-picker-header",
    navButton: "my-picker-nav",
    title: "my-picker-title",
    monthSelect: "my-picker-select",
    yearSelect: "my-picker-select",
    weekdayRow: "my-picker-weekdays",
    weekday: "my-picker-weekday",
    grid: "my-picker-grid",
    day: "my-picker-day",
    daySelected: "my-picker-day-selected",
    dayToday: "my-picker-day-today",
    dayWeekend: "my-picker-day-weekend",
    dayHoliday: "my-picker-day-holiday",
    dayDisabled: "my-picker-day-disabled",
    legend: "my-picker-legend",
    legendItem: "my-picker-legend-item",
    adLine: "my-picker-ad-line",
    empty: "my-picker-empty",
  }}
/>
```

### Locale and Digits

```tsx
// Nepali month and weekday names with Devanagari digits
<BsDatePicker {...props} locale="ne" />

// Nepali labels with Latin digits
<BsDatePicker {...props} locale="ne" nepaliDigits={false} />

// English labels and Latin digits
<BsDatePicker {...props} locale="en" nepaliDigits={false} />
```

### Date Restrictions and Display

```tsx
<BsDatePicker
  value={value}
  onChange={(selection) => setValue(selection?.bs ?? null)}
  initialMonth={[2082, 4]}
  min="2082-04-01"
  max="2082-06-32"
  disabledDates={["2082-04-20", "2082-05-05"]}
  disabledWeekdays={[6]}
  showAdLine={false}
/>
```

Weekday numbers use JavaScript conventions: `0` is Sunday and `6` is Saturday.

### Bring Your Own Trigger

Use `BsDatePickerPopup` when the application should own the trigger and popup state:

```tsx
import { useState } from "react";
import { BsDatePickerPopup } from "nepali-calendar-toolkit/react";

export function CustomTriggerDatePicker() {
  const [open, setOpen] = useState(false);
  const [value, setValue] = useState<string | null>("2082-04-15");

  return (
    <div className="date-field">
      <button type="button" onClick={() => setOpen(true)}>
        {value ?? "Select date"}
      </button>

      <BsDatePickerPopup
        open={open}
        value={value}
        onOpenChange={setOpen}
        onChange={(selection) => setValue(selection?.bs ?? null)}
        locale="en"
        className="custom-date-popup"
      />
    </div>
  );
}
```

The `/react` entry point also exports `BsDateTrigger`, `createBsPickerController`, and `isBsKeyDisabled` for custom React controls. The package does not currently include ready-made Vue, Angular, or Svelte components.

## Build a Custom Calendar View

Use the framework-independent month-grid helpers with plain JavaScript, React, Vue, Angular, Svelte, or any other UI. These helpers provide the calendar data; your application renders the picker interface.

```ts
import {
  ready,
  buildMonthGrid,
  daysInBsMonth,
  monthNames,
  formatBsLong,
} from "nepali-calendar-toolkit";

await ready;

const grid = await buildMonthGrid(2083, 6, "2083-06-04");

console.log(monthNames("ne")[5]);       // आश्विन
console.log(daysInBsMonth(2083, 6));   // Number of days in the month
console.log(formatBsLong("2083-06-04", "en"));
console.log(grid.leadingBlanks);       // Weekday position of day 1
console.log(grid.cells[0]);
```

Each day cell includes its BS day, BS key, AD date, weekday, weekend status, optional holiday name, and today status. Use these values to render any calendar design you want.

## Notes

- Nepali months use numbers `1` (Baisakh) through `12` (Chaitra).
- A Nepali fiscal year starts on Shrawan 1 and ends at the end of Ashadh in the following BS year.
- Requests outside the supported calendar or holiday range throw an error.
