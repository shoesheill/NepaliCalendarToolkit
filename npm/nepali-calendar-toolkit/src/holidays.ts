import { isSupportedYear, monthLengths, holidaysFor } from "./calendarData";
import { convertToAd, convertToNepali, supportedYearsError } from "./converter";
import { addDays, dayOfWeekName, formatDateString, parseDateString } from "./dateHelper";
import { getMonthDateInAd } from "./ranges";
import { isWeekend, getWeekendDays, setWeekendDays } from "./weekend";
import { HolidayInfo, HolidayOrWeekend, NepaliDate } from "./types";

export function configureWeekendDays(...days: number[]): void {
  setWeekendDays(...days);
}

export function getConfiguredWeekendDays(): number[] {
  return getWeekendDays();
}

export async function getHolidaysAndWeekends(
  yearBs: number,
  month?: number,
  returnType: HolidayOrWeekend = "both",
  isFiscalYear = false,
): Promise<HolidayInfo[]> {
  if (isFiscalYear && !month) {
    // Entire fiscal year: Shrawan 1 (yearBs) → last Ashar (yearBs + 1)
    if (!isSupportedYear(yearBs) || !isSupportedYear(yearBs + 1)) throw supportedYearsError();
    const rangeStart = parseDateString(await convertToAd(new NepaliDate(yearBs, 4, 1)));
    const rangeEnd = parseDateString(await convertToAd(new NepaliDate(yearBs + 1, 3, monthLengths[yearBs + 1][2])));
    return getHolidaysAndWeekendsInRange(rangeStart, rangeEnd, returnType);
  }

  const actualYear = isFiscalYear && month ? (month >= 4 ? yearBs : yearBs + 1) : yearBs;
  if (!isSupportedYear(actualYear)) throw supportedYearsError();

  const includeHolidays = returnType === "holidays" || returnType === "both";
  const includeWeekends = returnType === "weekends" || returnType === "both";
  const results: HolidayInfo[] = [];

  if (includeHolidays) {
    const holidayList = await holidaysFor(actualYear);
    for (const holiday of holidayList) {
      if (month == null || holiday.month === month) {
        const nepaliDate = new NepaliDate(actualYear, holiday.month, holiday.day);
        const adDate = await convertToAd(nepaliDate);
        results.push({
          dayName: dayOfWeekName(parseDateString(adDate)),
          holidayName: holiday.name,
          adDate,
          bsDate: nepaliDate.toString(),
        });
      }
    }
  }

  if (includeWeekends) {
    const months = month != null ? [month] : [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    for (const m of months) {
      const { startDate, endDate } = await getMonthDateInAd(actualYear, m);
      for (let date = parseDateString(startDate); formatDateString(date) <= endDate; date = addDays(date, 1)) {
        if (isWeekend(date.getUTCDay())) {
          const nepaliDate = await convertToNepali(date);
          results.push({
            dayName: dayOfWeekName(date),
            holidayName: "",
            adDate: formatDateString(date),
            bsDate: nepaliDate.toString(),
          });
        }
      }
    }
  }

  return results.sort((a, b) => a.adDate.localeCompare(b.adDate));
}

async function getHolidaysAndWeekendsInRange(
  rangeStart: Date,
  rangeEnd: Date,
  returnType: HolidayOrWeekend,
): Promise<HolidayInfo[]> {
  const results: HolidayInfo[] = [];
  const includeHolidays = returnType === "holidays" || returnType === "both";
  const includeWeekends = returnType === "weekends" || returnType === "both";
  const holidayCache = new Map<number, Awaited<ReturnType<typeof holidaysFor>>>();
  const end = formatDateString(rangeEnd);

  for (let date = rangeStart; formatDateString(date) <= end; date = addDays(date, 1)) {
    const nepaliDate = await convertToNepali(date);
    if (!isSupportedYear(nepaliDate.year)) continue;

    if (includeHolidays) {
      if (!holidayCache.has(nepaliDate.year)) {
        holidayCache.set(nepaliDate.year, await holidaysFor(nepaliDate.year));
      }
      const holiday = holidayCache
        .get(nepaliDate.year)!
        .find((h) => h.month === nepaliDate.month && h.day === nepaliDate.day);
      if (holiday) {
        results.push({
          dayName: dayOfWeekName(date),
          holidayName: holiday.name,
          adDate: formatDateString(date),
          bsDate: nepaliDate.toString(),
        });
        continue;
      }
    }

    if (includeWeekends && isWeekend(date.getUTCDay())) {
      results.push({
        dayName: dayOfWeekName(date),
        holidayName: "",
        adDate: formatDateString(date),
        bsDate: nepaliDate.toString(),
      });
    }
  }

  return results.sort((a, b) => a.adDate.localeCompare(b.adDate));
}
