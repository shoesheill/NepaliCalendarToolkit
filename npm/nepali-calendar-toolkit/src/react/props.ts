// ─── Shared prop contract — framework-free ───────────────────────────────────
// One vocabulary for the React wrapper AND any future Vue/Angular/Svelte
// wrapper: what goes in (options), what comes out (selection) and the
// locale/devanagari + styling switches. No DOM, no JSX here — so every
// framework package can import the same contract without a React dependency.

// The BS vocabulary itself — date string, locale, weekday labels, Devanagari
// digits, key normalisation — is owned by the core entry point and re-exported
// here, so a host app and every framework wrapper share ONE contract instead of
// two copies drifting apart.
import {
  BS_WEEKDAY_LABELS,
  NEPALI_DIGITS,
  monthNames,
  normalizeBsKey,
  toPickerDigits,
  type BsDateString,
  type BsLocale,
  type BsWeekdayLabels,
} from "../index";

export { BS_WEEKDAY_LABELS, NEPALI_DIGITS, normalizeBsKey, toPickerDigits };
export type { BsDateString, BsLocale, BsWeekdayLabels };

/** A single BS date + its resolved AD equivalent. */
export interface BsDateValue {
  /** BS year, e.g. 2082. */
  year: number;
  /** BS month 1–12 (1 = Baisakh). */
  month: number;
  /** BS day 1–32. */
  day: number;
}

export interface BsDateSelection {
  /** BS date, zero-padded: "2082-04-15". */
  bs: BsDateString;
  /** Structured BS value. */
  value: BsDateValue;
  /** AD equivalent, yyyy-MM-dd. */
  ad: string;
  /** Holiday name from `Holidays/<year>.json`, if any. */
  holidayName?: string;
  /** True when the day falls on the configured weekend set. */
  weekend: boolean;
}

export interface BsDatePickerOptions {
  /** Controlled value (BS yyyy-MM-dd). Omit for uncontrolled. */
  value?: BsDateString | null;
  /** Controlled popup visibility. Omit for uncontrolled. */
  open?: boolean;
  /** Initial month as [year, month]; defaults to value, then today. */
  initialMonth?: readonly [number, number];
  min?: BsDateString;
  max?: BsDateString;
  disabledDates?: BsDateString[];
  disabledWeekdays?: number[];
  locale?: BsLocale;
  /** Render BS digits in Devanagari (१२३…) instead of 123. Defaults to `true` when `locale` is "ne"; pass `false` to force Latin digits. */
  nepaliDigits?: boolean;
  /** Show the AD equivalent line under the grid. @default true */
  showAdLine?: boolean;
  /** Extra CSS class(es) for the popup root. Styling hooks below let the host theme without forking. */
  className?: string;
  classNames?: {
    root?: string;
    header?: string;
    navButton?: string;
    title?: string;
    /** Month `<select>` in the header (rendered before the year one). */
    monthSelect?: string;
    yearSelect?: string;
    weekdayRow?: string;
    weekday?: string;
    grid?: string;
    day?: string;
    daySelected?: string;
    dayToday?: string;
    dayWeekend?: string;
    dayHoliday?: string;
    dayDisabled?: string;
    /** Legend row under the grid explaining the red digit cue. */
    legend?: string;
    legendItem?: string;
    adLine?: string;
    empty?: string;
  };
  /** Placeholder when nothing is selected. */
  placeholder?: string;
  /** Disable the whole control. */
  disabled?: boolean;
  /** Fires on every committed day pick. */
  onChange?: (selection: BsDateSelection | null) => void;
  /** Fires when the popup opens/closes (uncontrolled mode drives it internally). */
  onOpenChange?: (open: boolean) => void;
}

export interface BsTriggerOptions {
  value?: BsDateString | null;
  locale?: BsLocale;
  /** Devanagari digits; same locale default as `BsDatePickerOptions.nepaliDigits`. */
  nepaliDigits?: boolean;
  placeholder?: string;
  disabled?: boolean;
  className?: string;
  classNames?: {
    root?: string;
    icon?: string;
    label?: string;
  };
}

// BS_WEEKDAY_LABELS, NEPALI_DIGITS, toPickerDigits, normalizeBsKey and the
// BsWeekdayLabels type are re-exported from `nepali-calendar-toolkit` at the
// top of this file — the toolkit owns that vocabulary.

/** Parses a BS key into its structured value; null when malformed. */
export function parseBsKey(key: string | null | undefined): BsDateValue | null {
  const norm = normalizeBsKey(key);
  if (!norm) return null;
  const [year, month, day] = norm.split("-").map(Number);
  return { year, month, day };
}

/**
 * Locale-aware "15 Shrawan 2082" label for a BS key (falls back to the key).
 * Digits follow the locale — Devanagari for "ne" — unless `nepaliDigits` says
 * otherwise (an explicit `false` keeps Latin digits even under "ne").
 */
export function formatBsLabel(
  key: string | null | undefined,
  locale: BsLocale = "en",
  nepaliDigits: boolean = locale === "ne",
): string {
  const parsed = parseBsKey(key);
  if (!parsed) return key ?? "";
  const names = monthNames(locale);
  const day = toPickerDigits(parsed.day, nepaliDigits);
  const year = toPickerDigits(parsed.year, nepaliDigits);
  return `${day} ${names[parsed.month - 1] ?? parsed.month} ${year}`;
}
