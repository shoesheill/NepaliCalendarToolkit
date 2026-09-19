// ─── BsDatePicker option + selection types ───────────────────────────────────
// Small shared vocabulary for the datepicker element and any framework
// wrappers: what goes in (options), what comes out (selection) and the
// locale/devanagari switches. Framework-free — no DOM types here.

/** BS date string in yyyy-MM-dd form, e.g. "2082-04-15". */
export type BsDateString = string;

/** Calendar locale: English or Nepali month/day names. */
export type BsLocale = "en" | "ne";

export interface BsDateSelection {
  /** BS date, zero-padded: "2082-04-15". */
  bs: BsDateString;
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
  /** Initial month as [year, month]; defaults to value, then today. */
  initialMonth?: readonly [number, number];
  min?: BsDateString;
  max?: BsDateString;
  disabledDates?: BsDateString[];
  disabledWeekdays?: number[];
  locale?: BsLocale;
  /** Render BS digits in Devanagari (१२३…) instead of 123. */
  nepaliDigits?: boolean;
  /** Extra CSS class(es) for the host element. */
  className?: string;
  /** Placeholder when nothing is selected. */
  placeholder?: string;
  /** Disable the whole control. */
  disabled?: boolean;
  /** Fires on every committed day pick. */
  onChange?: (selection: BsDateSelection | null) => void;
  /** Fires when the popup opens/closes. */
  onOpenChange?: (open: boolean) => void;
}

/** Weekday header labels, Sunday-first to match the 6×7 grid. */
export interface BsWeekdayLabels {
  en: readonly [string, string, string, string, string, string, string];
  ne: readonly [string, string, string, string, string, string, string];
}

export const BS_WEEKDAY_LABELS: BsWeekdayLabels = {
  en: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
  ne: ["आइत", "सोम", "मंगल", "बुध", "बिही", "शुक्र", "शनि"],
};

/** Devanagari digits ०–९ for the nepaliDigits switch. */
export const NEPALI_DIGITS = ["०", "१", "२", "३", "४", "५", "६", "७", "८", "९"];

/** Renders 1–31 with Devanagari digits when enabled. */
export function toPickerDigits(day: number, nepaliDigits: boolean): string {
  if (!nepaliDigits) return String(day);
  return String(day)
    .split("")
    .map((ch) => NEPALI_DIGITS[Number(ch)] ?? ch)
    .join("");
}

/**
 * Normalises a BS key to zero-padded "yyyy-MM-dd" for stable comparison.
 * Returns null for anything that is not shaped like a BS key.
 */
export function normalizeBsKey(key: string | null | undefined): BsDateString | null {
  if (!key) return null;
  const parts = key.split("-").map(Number);
  if (parts.length !== 3 || parts.some((n) => !Number.isInteger(n))) return null;
  const [y, m, d] = parts;
  if (m < 1 || m > 12 || d < 1 || d > 32) return null;
  return `${y}-${String(m).padStart(2, "0")}-${String(d).padStart(2, "0")}`;
}
