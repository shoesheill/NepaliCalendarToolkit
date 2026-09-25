// ─── Month grid model ────────────────────────────────────────────────────────
// Framework-free view model for one BS month: leading padding, day cells and
// trailing pad so every month renders a stable 6×7 grid. Events resolve
// from the year's `Events/<year>.json`; weekends from the configurable set
// (default Saturday + Sunday, JS day numbers). Everything here is plain data —
// no DOM — so any framework (or none) can render it.

import { getEvents } from "../dataProvider";
import { knownMonths, monthLengths } from "../calendarData";
import { convertToAd, MONTH_NAMES_EN, MONTH_NAMES_NP } from "../converter";
import { formatDateString, parseDateString } from "../dateHelper";
import { isWeekend } from "../weekend";

export interface BsDayCell {
  /** BS day number, 1-based. */
  day: number;
  /** Full BS key, e.g. "2082-04-15". */
  bsKey: string;
  /** AD equivalent (yyyy-MM-dd). */
  adDate: string;
  /** JS day number 0=Sun…6=Sat of the AD equivalent. */
  weekday: number;
  /** True for Saturday/Sunday (or the configured weekend set). */
  weekend: boolean;
  /** Rich event records from `Events/<year>.json`, if any. */
  events: import("../types").CalendarEvent[];
  /** True when the day equals the supplied `today` key. */
  today: boolean;
}

export interface BsMonthGrid {
  year: number;
  month: number;
  /** 0=Sun…6=Sat weekday of day 1 (AD equivalent). */
  leadingBlanks: number;
  cells: BsDayCell[];
}

/** English + Nepali month names, matching the converter's canonical lists. */
export function monthNames(locale: "en" | "ne" = "en"): readonly string[] {
  return locale === "ne" ? MONTH_NAMES_NP : MONTH_NAMES_EN;
}

/** Days in a BS month from the published month lengths; null when unknown. */
export function daysInBsMonth(year: number, month: number): number | null {
  if (month < 1 || month > 12) return null;
  if (month > knownMonths(year)) return null;
  return monthLengths[year][month - 1] ?? null;
}

/**
 * Builds the grid for one BS month. Resolves leading blanks from the AD
 * weekday of day 1 and tags each cell with weekend + event flags. Events
 * failing to load (a year with no `Events/<year>.json`) degrade to
 * weekend-only marking rather than throwing.
 */
export async function buildMonthGrid(
  year: number,
  month: number,
  todayBsKey?: string,
): Promise<BsMonthGrid> {
  const length = daysInBsMonth(year, month);
  if (length == null) throw new Error(`BS month ${year}-${month} is not published yet`);

  const firstAd = await convertToAd({ year, month, day: 1 } as never);
  const leadingBlanks = parseDateString(firstAd).getUTCDay();

  let events: Map<string, import("../types").CalendarEvent[]>;
  try {
    const list = await getEvents(year);
    events = new Map();
    for (const event of list.filter((e) => e.bsMonth === month)) {
      const key = `${year}-${month}-${event.bsDay}`;
      events.set(key, [...(events.get(key) ?? []), event]);
    }
  } catch {
    events = new Map();
  }

  const cells: BsDayCell[] = [];
  for (let day = 1; day <= length; day++) {
    const bsKey = `${year}-${month}-${day}`;
    const adDate = day === 1 ? firstAd : await convertToAd({ year, month, day } as never);
    const weekday = parseDateString(adDate).getUTCDay();
    cells.push({
      day,
      bsKey,
      adDate,
      weekday,
      weekend: isWeekend(weekday),
      events: events.get(bsKey) ?? [],
      today: todayBsKey === bsKey,
    });
  }

  return { year, month, leadingBlanks, cells };
}

/** Formats a BS key the way humans read it: "15 Shrawan 2082". */
export function formatBsLong(bsKey: string, locale: "en" | "ne" = "en"): string {
  const [y, m, d] = bsKey.split("-").map(Number);
  const names = monthNames(locale);
  return `${d} ${names[m - 1] ?? m} ${y}`;
}

/** Today's BS key (yyyy-M-d) from an AD instant, or undefined when unsupported. */
export async function todayBsKey(now = new Date()): Promise<string | undefined> {
  try {
    const { convertToNepali } = await import("../converter");
    const { toKathmanduDateString, parseDateString: parse } = await import("../dateHelper");
    const local = toKathmanduDateString(now);
    const bs = await convertToNepali(parse(local));
    return `${bs.year}-${bs.month}-${bs.day}`;
  } catch {
    return undefined;
  }
}

/** Stable equality for BS keys regardless of zero-padding. */
export function sameBsDay(a: string | null | undefined, b: string | null | undefined): boolean {
  if (!a || !b) return false;
  const norm = (k: string) => k.split("-").map(Number).join("-");
  return norm(a) === norm(b);
}

/** Checks an AD yyyy-MM-dd string is a real calendar date. */
export function isAdKey(key: string): boolean {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(key)) return false;
  const d = parseDateString(key);
  return formatDateString(d) === key;
}
