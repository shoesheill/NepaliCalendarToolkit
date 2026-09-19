# nepali-datepicker-react

React popup datepicker for Bikram Sambat dates, built on
[`nepali-calendar-toolkit`](https://www.npmjs.com/package/nepali-calendar-toolkit) — BS month grid
with holidays, weekends, min/max bounds and Devanagari digits.

- **Headless core** (`createBsPickerController`) is framework-free, so Vue/Angular/Svelte wrappers can reuse it.
- **Zero styling lock-in**: plain `nct-*` CSS defaults plus `classNames` hooks (tailwind class strings work as-is).
- Weekends and holidays come from the toolkit, so both stay correct as the calendar data is refreshed.

## Install

```bash
npm install nepali-datepicker-react nepali-calendar-toolkit react
```

## Usage

```tsx
import { BsDatePicker } from "nepali-datepicker-react";
import "nepali-datepicker-react/styles.css";

<BsDatePicker
  value={value}                 // "2082-04-15" (BS, yyyy-MM-dd)
  onChange={(s) => setValue(s?.bs ?? null)}
  onOpenChange={setOpen}
  min="2082-04-01"
  max="2082-04-32"
  locale="ne"                   // month/weekday names
  classNames={{ daySelected: "bg-purple-600 text-white" }}
/>
```

Bring your own trigger (input/button) and render only the popup:

```tsx
import { BsDatePickerPopup } from "nepali-datepicker-react";

<BsDatePickerPopup
  open={open}
  onOpenChange={setOpen}
  value={value}
  onChange={(s) => { setValue(s?.bs ?? null); setOpen(false); }}
/>
```

### Header dropdowns

The popup header reads **month then year** — `Baisakh ▾ 2083 ▾` — matching the
"month year" order used by shadcn/MUI/react-day-picker and how the title is read.
Both are native `<select>`s:

- **Month** lists all 12 BS months with localised names (`locale="ne"` → श्रावण).
  Months a year has no data for yet are `disabled`, so a half-seeded year can't
  open an empty panel. Changing it swaps the grid and leaves the year alone.
- **Year** lists the published window from `supportedYears()`.

`‹` / `›` still step one month and now skip over unpublished months instead of
dead-ending on "This month is not published yet". Switching year keeps the month
when the new year publishes it, otherwise falls back to that year's last
published month. `classNames.monthSelect` / `classNames.yearSelect` theme them.

### Holiday marking

Public holidays and weekly offs are marked with **red digits only** — no dot
marker under the number. Because the two share one visual cue, the holiday itself
is surfaced without a glyph:

- every day button carries an `aria-label` of `day, weekday, holiday name` and a
  `title` of the holiday name, so the holiday reaches both screen readers and the
  hover tooltip;
- `nct-hol` / `classNames.dayHoliday` still distinguishes holidays from plain
  weekends for anyone who wants a different treatment in their own CSS;
- a one-line legend under the grid spells the red cue out. Text follows `locale`
  (`Public holiday / weekly off`, `सार्वजनिक बिदा / साप्ताहिक बिदा`) and the row is
  themeable via `classNames.legend` / `classNames.legendItem`.

### onChange payload

```ts
{ bs: "2082-04-15", value: { year: 2082, month: 4, day: 15 },
  ad: "2026-07-31", holidayName?: "…", weekend: false }
```

## Exports

| Export | Purpose |
| --- | --- |
| `BsDatePicker` | Trigger + popup (batteries included) |
| `BsDatePickerPopup` | Popup only — host keeps its own trigger |
| `BsDateTrigger` | Default trigger button |
| `createBsPickerController` | Headless view-state machine (no JSX) |
| `isBsKeyDisabled` | min/max + disabled dates/weekdays check |
| `normalizeBsKey`, `parseBsKey`, `formatBsLabel` | BS key helpers |

## Digits

BS numbers — grid days, year options, the trigger label, the `वि.सं.` line —
follow the locale: `locale="ne"` renders Devanagari (१५ श्रावण २०८२), `"en"`
renders Latin. `nepaliDigits` overrides that either way, and an explicit `false`
under `"ne"` keeps Nepali names with Latin digits:

```tsx
<BsDatePicker locale="ne" />                        {/* १५ श्रावण २०८२ */}
<BsDatePicker locale="ne" nepaliDigits={false} />   {/* 15 श्रावण 2082 */}
```

Gregorian AD dates (the `AD 2026-07-31` confirmation and cell tooltips) always
stay Latin, and `<option value>`s always carry numeric values so the controller
is unaffected.

## Styling

`import "nepali-datepicker-react/styles.css"` for the default look, or skip it and style
via `classNames` / the documented `--nct-*` CSS variables. `nct-*` classes are always applied,
so a host stylesheet can override them.

## Development

```bash
npm run build     # tsc + dist/styles.css
npm test          # build + controller smoke + jsdom popup render test
```

`nepali-calendar-toolkit` is linked from `../nepali-calendar-toolkit` for development, so build the
toolkit first when working inside the repository.
