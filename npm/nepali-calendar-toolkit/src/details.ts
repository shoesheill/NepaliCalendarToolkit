import { convertToNepali, convertToNepaliFromString } from "./converter";
import { getDayDetails as loadDayDetails, getEvents as loadEvents } from "./dataProvider";
import { dayOfWeekName, parseDateString } from "./dateHelper";
import {
  BS_MONTH_NAMES_NP,
  CHANDRAMA_NAMES_EN,
  CHANDRAMA_NAMES_NP,
  LUNAR_MONTH_NAMES_EN,
  LUNAR_MONTH_NAMES_NP,
  NS_MONTH_NAMES_NP,
  PAKSHA_NAMES_EN,
  PAKSHA_NAMES_NP,
  TITHI_NAMES_EN,
  TITHI_NAMES_NP,
  nsMonthNumber,
  pakshaKey,
} from "./lookups";
import { CalendarDayInfo, CalendarEvent, DayDetailsData, NepaliDate } from "./types";

function validYear(year: number): void {
  if (!Number.isInteger(year) || year < 1) throw new Error(`Invalid BS year: ${year}`);
}

function toDayInfo(raw: DayDetailsData, year: number): CalendarDayInfo {
  const adDate = raw.adDate;
  const bsMonthNamesEn = [
    "Baisakh", "Jestha", "Ashadh", "Shrawan", "Bhadra", "Ashwin",
    "Kartik", "Mangsir", "Poush", "Magh", "Falgun", "Chaitra",
  ];
  return {
    ...raw,
    bsDate: new NepaliDate(year, raw.bsMonth, raw.bsDay),
    bsMonthName: BS_MONTH_NAMES_NP[raw.bsMonth - 1],
    bsMonthNameEn: bsMonthNamesEn[raw.bsMonth - 1],
    dayOfWeek: dayOfWeekName(parseDateString(adDate)),
    tithiName: raw.tithi == null ? undefined : TITHI_NAMES_NP[raw.tithi],
    tithiNameEn: raw.tithi == null ? undefined : TITHI_NAMES_EN[raw.tithi],
    lunarMonthName: raw.chandrama == null ? undefined : LUNAR_MONTH_NAMES_NP[raw.chandrama],
    lunarMonthNameEn: raw.chandrama == null ? undefined : LUNAR_MONTH_NAMES_EN[raw.chandrama],
    paksha: pakshaKey(raw.chandrama) ? PAKSHA_NAMES_NP[pakshaKey(raw.chandrama)!] : undefined,
    pakshaEn: pakshaKey(raw.chandrama) ? PAKSHA_NAMES_EN[pakshaKey(raw.chandrama)!] : undefined,
    nsMonthName: NS_MONTH_NAMES_NP[nsMonthNumber(raw.nsMonth) ?? -1],
  };
}

/** Gets the stored daily details for a BS date. */
export async function getDayDetails(date: NepaliDate): Promise<CalendarDayInfo> {
  validYear(date.year);
  const list = await loadDayDetails(date.year);
  const raw = list.find((item) => item.bsMonth === date.month && item.bsDay === date.day);
  if (!raw) throw new Error(`Daily details for ${date} are not available`);
  return toDayInfo(raw, date.year);
}

/** Gets the stored daily details for an AD yyyy-MM-dd date. */
export async function getDayDetailsFromAd(adDate: string): Promise<CalendarDayInfo> {
  const bs = await convertToNepaliFromString(adDate);
  return getDayDetails(bs);
}

/** Gets all rich event occurrences for a BS year/month. */
export async function getEvents(
  yearBs: number,
  month?: number,
  governmentHolidaysOnly = false,
): Promise<CalendarEvent[]> {
  validYear(yearBs);
  if (month != null && (month < 1 || month > 12)) throw new Error("Month must be between 1 and 12");
  const events = await loadEvents(yearBs);
  return events
    .filter((event) =>
      (month == null || event.bsMonth === month) &&
      (!governmentHolidaysOnly || event.isGovernmentHoliday),
    )
    .sort((a, b) => a.adDate.localeCompare(b.adDate) || a.nameEn?.localeCompare(b.nameEn ?? "") || 0);
}

/** Gets all rich event occurrences for one BS date. */
export async function getEventsForDate(
  date: NepaliDate,
  governmentHolidaysOnly = false,
): Promise<CalendarEvent[]> {
  return (await getEvents(date.year, date.month, governmentHolidaysOnly))
    .filter((event) => event.bsDay === date.day);
}

export {
  BS_MONTH_NAMES_NP,
  TITHI_NAMES_EN,
  TITHI_NAMES_NP,
  CHANDRAMA_NAMES_EN,
  CHANDRAMA_NAMES_NP,
  LUNAR_MONTH_NAMES_EN,
  LUNAR_MONTH_NAMES_NP,
  PAKSHA_NAMES_EN,
  PAKSHA_NAMES_NP,
  NS_MONTH_NAMES_NP,
};