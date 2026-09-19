// --- Headless popup state - framework-free ---
// All async calendar logic without JSX: view-month seeding, one-grid-per-view
// loading with stale-response guards, year-window clamping, min/max checks.
// React wrapper (BsDatePicker.tsx) and future Vue/Angular wrappers are thin
// render layers over this controller.

import {
  buildMonthGrid,
  convertToAd,
  getCurrentDateInfo,
  isSupportedMonth,
  knownMonths,
  monthNames,
  sameBsDay,
  supportedYears,
  type BsMonthGrid,
} from "../index";
import {
  BS_WEEKDAY_LABELS,
  normalizeBsKey,
  parseBsKey,
  type BsDateSelection,
  type BsDateString,
  type BsLocale,
} from "./props";

export type { BsMonthGrid };

export interface BsPickerControllerOptions {
  value?: BsDateString | null;
  initialMonth?: readonly [number, number];
  min?: BsDateString;
  max?: BsDateString;
  disabledDates?: BsDateString[];
  disabledWeekdays?: number[];
  locale?: BsLocale;
  onChange?: (selection: BsDateSelection | null) => void;
}

export interface BsPickerViewState {
  viewYear: number | null;
  viewMonth: number | null;
  grid: BsMonthGrid | null;
  loading: boolean;
  years: number[];
  /** Selectable BS months, 1–12 in calendar order (1 = Baisakh). */
  months: number[];
  /** Localised month names, index-aligned with 1–12. */
  monthNames: readonly string[];
  /** Months of `viewYear` that have no published data yet — render as disabled. */
  disabledMonths: number[];
  monthLabel: string;
  weekdayLabels: readonly string[];
  todayKey: string | null;
}

export function isBsKeyDisabled(
  key: BsDateString | null,
  opts: Pick<BsPickerControllerOptions, "min" | "max" | "disabledDates" | "disabledWeekdays">,
  weekdayOf?: (key: BsDateString) => number | undefined,
): boolean {
  if (!key) return true;
  const norm = normalizeBsKey(key);
  if (!norm) return true;
  const min = normalizeBsKey(opts.min);
  const max = normalizeBsKey(opts.max);
  if (min && norm < min) return true;
  if (max && norm > max) return true;
  if (opts.disabledDates?.some((d) => sameBsDay(d, norm))) return true;
  if (opts.disabledWeekdays?.length && weekdayOf) {
    const wd = weekdayOf(norm);
    if (wd != null && opts.disabledWeekdays.includes(wd)) return true;
  }
  return false;
}

export function createBsPickerController(
  initial: BsPickerControllerOptions,
  onState: (state: BsPickerViewState) => void,
): {
  setOptions: (opts: BsPickerControllerOptions) => void;
  open: () => void;
  stepMonth: (delta: number) => void;
  setYear: (year: number) => void;
  setMonth: (month: number) => void;
  pickDay: (day: number) => Promise<void>;
  dispose: () => void;
} {
  let opts = initial;
  let viewYear: number | null = null;
  let viewMonth: number | null = null;
  let grid: BsMonthGrid | null = null;
  let loading = false;
  let todayKey: string | null = null;
  let requestId = 0;
  let disposed = false;

  const yearsOf = (): number[] => {
    try { return supportedYears(); } catch { return []; }
  };

  /**
   * Months of `year` that have calendar data. Empty array means "no info" (data
   * not loaded yet) rather than "nothing published", so callers must treat it as
   * "unknown — allow everything".
   */
  const publishedMonths = (year: number): number[] => {
    try {
      const count = knownMonths(year);
      if (!count) return [];
      const out: number[] = [];
      for (let m = 1; m <= Math.min(count, 12); m += 1) {
        if (isSupportedMonth(year, m)) out.push(m);
      }
      return out;
    } catch { return []; }
  };

  const isPublished = (year: number, month: number): boolean => {
    const list = publishedMonths(year);
    return list.length === 0 || list.includes(month);
  };

  /** Last published month of `year`, or `month` when nothing is known. */
  const fallbackMonth = (year: number, month: number): number => {
    if (isPublished(year, month)) return month;
    const list = publishedMonths(year);
    return list.length > 0 ? list[list.length - 1] : month;
  };

  const emit = (): void => {
    if (disposed) return;
    const locale: BsLocale = opts.locale ?? "en";
    let names: readonly string[] = [];
    let monthLabel = "\u2014";
    try { names = monthNames(locale); } catch { names = []; }
    if (viewMonth != null) {
      monthLabel = names[viewMonth - 1] ?? String(viewMonth);
    }
    const ALL_MONTHS = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    const published = viewYear != null ? publishedMonths(viewYear) : [];
    onState({
      viewYear, viewMonth, grid, loading,
      years: yearsOf(),
      months: ALL_MONTHS,
      monthNames: names,
      disabledMonths: published.length > 0 ? ALL_MONTHS.filter((m) => !published.includes(m)) : [],
      monthLabel,
      weekdayLabels: BS_WEEKDAY_LABELS[locale] ?? BS_WEEKDAY_LABELS.en,
      todayKey,
    });
  };

  const loadGrid = async (year: number, month: number): Promise<void> => {
    const id = ++requestId;
    loading = true;
    emit();
    try {
      const today = await getCurrentDateInfo().catch(() => null);
      if (id !== requestId || disposed) return;
      todayKey = today ? `${today.year}-${today.month}-${today.day}` : null;
      const g = await buildMonthGrid(year, month, todayKey ?? undefined);
      if (id !== requestId || disposed) return;
      grid = g;
    } catch {
      if (id !== requestId || disposed) return;
      grid = null;
    } finally {
      if (id === requestId && !disposed) { loading = false; emit(); }
    }
  };

  const clampYear = (y: number): number | null => {
    const years = yearsOf();
    if (years.length === 0) return y;
    if (y < years[0] || y > years[years.length - 1]) return null;
    return y;
  };

  return {
    setOptions(next: BsPickerControllerOptions): void { opts = next; emit(); },
    open(): void {
      if (viewYear != null && viewMonth != null) { emit(); return; }
      const fromValue = parseBsKey(opts.value ?? null);
      if (fromValue) {
        viewYear = fromValue.year; viewMonth = fromValue.month;
        emit(); void loadGrid(viewYear, viewMonth); return;
      }
      if (opts.initialMonth) {
        viewYear = opts.initialMonth[0]; viewMonth = opts.initialMonth[1];
        emit(); void loadGrid(viewYear, viewMonth); return;
      }
      emit();
      getCurrentDateInfo().then((info) => {
        if (disposed) return;
        viewYear = info.year; viewMonth = info.month;
        emit(); void loadGrid(viewYear, viewMonth);
      }).catch(() => {
        if (disposed) return;
        const years = yearsOf();
        if (years.length > 0) {
          viewYear = years[years.length - 1]; viewMonth = 1;
          emit(); void loadGrid(viewYear, viewMonth);
        }
      });
    },
    stepMonth(delta: number): void {
      if (viewYear == null || viewMonth == null) return;
      let y = viewYear;
      let m = viewMonth;
      // Skip months with no published data so ‹ › never dead-ends on an empty
      // panel. A bounded walk keeps this safe when the year tail is unpublished.
      for (let i = 0; i < 24; i += 1) {
        m += delta;
        if (m < 1) { m = 12; y -= 1; } else if (m > 12) { m = 1; y += 1; }
        if (clampYear(y) == null) return;
        if (isPublished(y, m)) { viewYear = y; viewMonth = m; grid = null; emit(); void loadGrid(y, m); return; }
      }
    },
    setYear(year: number): void {
      if (viewMonth == null) return;
      if (clampYear(year) == null) return;
      viewYear = year;
      // Keep the month when the new year publishes it, otherwise land on the
      // last month that does (e.g. 2083 → 2084 when only Shrawan is out).
      viewMonth = fallbackMonth(year, viewMonth);
      grid = null;
      emit(); void loadGrid(viewYear, viewMonth);
    },
    setMonth(month: number): void {
      if (viewYear == null || viewMonth == null) return;
      if (!Number.isInteger(month) || month < 1 || month > 12) return;
      if (!isPublished(viewYear, month)) return;
      if (month === viewMonth) return;
      viewMonth = month; grid = null;
      emit(); void loadGrid(viewYear, month);
    },
    async pickDay(day: number): Promise<void> {
      if (viewYear == null || viewMonth == null || !grid) return;
      const cell = grid.cells.find((c) => c.day === day);
      if (!cell) return;
      const key = normalizeBsKey(cell.bsKey);
      if (!key) return;
      const g = grid;
      const weekdayOf = (k: BsDateString): number | undefined =>
        g.cells.find((c) => sameBsDay(c.bsKey, k))?.weekday;
      if (isBsKeyDisabled(key, opts, weekdayOf)) return;
      const parsed = parseBsKey(key);
      if (!parsed) return;
      try {
        const ad = await convertToAd({ year: parsed.year, month: parsed.month, day: parsed.day } as never);
        opts.onChange?.({ bs: key, value: parsed, ad, holidayName: cell.holidayName, weekend: cell.weekend });
      } catch { /* unpublished tail - keep old value */ }
    },
    dispose(): void { disposed = true; requestId++; },
  };
}
