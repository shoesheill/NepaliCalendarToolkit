import { getData } from "./dataProvider";
import { YearMeta } from "./types";

/**
 * Month lengths are a PREFIX, not necessarily twelve entries. Months publish one at a
 * time and the twelfth needs the FOLLOWING year's Baisakh 1, so a year in progress
 * legitimately holds only its first few. Check `isSupportedMonth`, not just the year.
 */
export let monthLengths: Record<number, number[]> = {};
export let yearStart: Record<number, string> = {};
/** Provenance per year; absent for data seeded before month-meta.json existed. */
export let yearMeta: Record<number, YearMeta> = {};

/** Must be awaited once (or awaited by the first library call) before conversions. */
export async function loadCalendarData(): Promise<void> {
  const [ml, ys, mm] = await Promise.all([
    getData<Record<number, number[]>>("month-lengths.json"),
    getData<Record<number, string>>("year-start.json"),
    getData<Record<number, YearMeta>>("month-meta.json"),
  ]);
  monthLengths = ml ?? {};
  yearStart = ys ?? {};
  yearMeta = mm ?? {};
}

export function supportedYears(): number[] {
  return Object.keys(monthLengths)
    .map(Number)
    .sort((a, b) => a - b);
}

export function isSupportedYear(year: number): boolean {
  return knownMonths(year) > 0;
}

/** Leading months known for a year; 0 when absent, 12 when closed. */
export function knownMonths(year: number): number {
  return monthLengths[year]?.length ?? 0;
}

/** Is this specific month resolvable? A partial year answers yes only up to its prefix. */
export function isSupportedMonth(year: number, month: number): boolean {
  return month >= 1 && month <= knownMonths(year);
}

/** Years complete enough to convert any date within them. */
export function completeYears(): number[] {
  return supportedYears().filter((y) => knownMonths(y) === 12);
}

/**
 * True when a year rests on an unverified prediction or is still incomplete.
 *
 * Nepal Patro publishes predictions ahead of the official gazette, and a revision can move
 * a date. Anything that has to be right — a tax filing, say — should refuse provisional
 * data; anything that merely displays a period is fine with it.
 */
export function isProvisional(year: number): boolean {
  const meta = yearMeta[year];
  // No metadata at all means data predating month-meta.json, which was only ever written
  // for confirmed, complete years — so treat a full year as final and a partial one as not.
  if (!meta) return knownMonths(year) !== 12;
  return !meta.verified || meta.knownMonths !== 12;
}

/** Full provenance for a year, or undefined when nothing is stored. */
export function getYearInfo(year: number): YearMeta | undefined {
  return yearMeta[year];
}
