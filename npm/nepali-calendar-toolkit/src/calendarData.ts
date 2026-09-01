import { getData, getHolidays } from "./dataProvider";
import { HolidayData } from "./types";

export let monthLengths: Record<number, number[]> = {};
export let yearStart: Record<number, string> = {};

/** Must be awaited once (or awaited by the first library call) before conversions. */
export async function loadCalendarData(): Promise<void> {
  const [ml, ys] = await Promise.all([
    getData<Record<number, number[]>>("month-lengths.json"),
    getData<Record<number, string>>("year-start.json"),
  ]);
  monthLengths = ml ?? {};
  yearStart = ys ?? {};
}

export async function holidaysFor(year: number): Promise<HolidayData[]> {
  return getHolidays(year);
}

export function supportedYears(): number[] {
  return Object.keys(monthLengths)
    .map(Number)
    .sort((a, b) => a - b);
}

export function isSupportedYear(year: number): boolean {
  return Object.prototype.hasOwnProperty.call(monthLengths, year);
}
