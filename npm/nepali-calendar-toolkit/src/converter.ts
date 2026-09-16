import { isSupportedMonth, isSupportedYear, knownMonths, monthLengths, supportedYears, yearStart } from "./calendarData";
import { addDays, formatDateString, parseDateString, toKathmanduDateString } from "./dateHelper";
import { NepaliDate } from "./types";

export const MONTH_NAMES_EN = [
  "Baisakh", "Jestha", "Ashadh", "Shrawan", "Bhadra", "Ashwin",
  "Kartik", "Mangsir", "Poush", "Magh", "Falgun", "Chaitra",
];
export const MONTH_NAMES_NP = [
  "बैशाख", "जेठ", "असार", "श्रावण", "भाद्र", "आश्विन",
  "कार्तिक", "मंसिर", "पौष", "माघ", "फाल्गुन", "चैत्र",
];

export function supportedYearsError(): Error {
  return new Error(
    `Year is outside the supported range. Supported years are: ${supportedYears().join(", ")}`,
  );
}

/** Finds which BS year an AD date (yyyy-MM-dd in Kathmandu) falls into. */
function getYear(localDate: string): number {
  const dates = Object.entries(yearStart).sort(([a], [b]) => Number(a) - Number(b));
  if (dates.length === 0) return -1;

  const lowerLimit = dates[0][1];
  for (const [key, yearStartDate] of dates) {
    if (localDate >= lowerLimit && localDate < yearStartDate) return Number(key) - 1;
  }

  const [lastYear, lastYearStart] = dates[dates.length - 1];
  const approxEnd = formatDateString(addDays(parseDateString(lastYearStart), 365));
  if (localDate >= lastYearStart && localDate < approxEnd) return Number(lastYear);

  return -1;
}

function getDaysPassed(year: number, localDate: string): number {
  const given = parseDateString(localDate).getTime();
  const start = parseDateString(yearStart[year]).getTime();
  return Math.round((given - start) / 86_400_000) + 1;
}

function getMonthAndDate(year: number, localDate: string): { month: number; day: number } {
  if (!isSupportedYear(year)) throw supportedYearsError();

  let daysPassed = getDaysPassed(year, localDate);
  const lengths = monthLengths[year];
  for (let i = 0; i < lengths.length; i++) {
    if (daysPassed <= lengths[i]) return { month: i + 1, day: daysPassed };
    daysPassed -= lengths[i];
  }
  // Falls beyond the months seeded so far — a real answer exists, it just is not
  // published yet, which is a different thing from the year being unsupported.
  throw new Error(
    `Date falls beyond the seeded months of BS ${year} (${lengths.length}/12 known).`,
  );
}

export function isValidNepaliDate(date: NepaliDate): boolean {
  if (!isSupportedMonth(date.year, date.month)) return false;
  return date.day >= 1 && date.day <= monthLengths[date.year][date.month - 1];
}

export function isValidNepaliMonth(year: number, month: number): boolean {
  return isSupportedMonth(year, month);
}

/** Converts an AD Date to a Nepali (BS) date. */
export async function convertToNepali(ad: Date): Promise<NepaliDate> {
  const localDate = toKathmanduDateString(ad);
  const npYear = getYear(localDate);
  if (npYear === -1 || !isSupportedYear(npYear)) throw supportedYearsError();

  const { month, day } = getMonthAndDate(npYear, localDate);
  return new NepaliDate(npYear, month, day);
}

/** Convenience overload accepting a yyyy-MM-dd string in AD. */
export async function convertToNepaliFromString(ad: string): Promise<NepaliDate> {
  return convertToNepali(parseDateString(ad));
}

/** Converts a Nepali (BS) date to an AD date (as a yyyy-MM-dd string). */
export async function convertToAd(nepaliDate: NepaliDate): Promise<string> {
  if (!isSupportedYear(nepaliDate.year)) throw supportedYearsError();
  // Only the months BEFORE the target need to be known — asking for Baisakh of a year
  // whose Kartik is unpublished is perfectly answerable.
  if (!isValidNepaliDate(nepaliDate)) throw new Error("Invalid Nepali date");

  let date = parseDateString(yearStart[nepaliDate.year]);
  for (let i = 0; i < nepaliDate.month - 1; i++) {
    date = addDays(date, monthLengths[nepaliDate.year][i]);
  }
  return formatDateString(addDays(date, nepaliDate.day - 1));
}
