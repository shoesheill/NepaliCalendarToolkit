import { isSupportedYear, monthLengths } from "./calendarData";
import { convertToNepali, MONTH_NAMES_EN, MONTH_NAMES_NP } from "./converter";
import { dayOfWeekName, parseDateString, toKathmanduDateString } from "./dateHelper";
import { isWeekend } from "./weekend";
import { CurrentDateInfo } from "./types";

export function getAvailableCalendarYearsBs(): { minYear: number; maxYear: number } {
  const years = Object.keys(monthLengths).map(Number).sort((a, b) => a - b);
  return years.length === 0 ? { minYear: 0, maxYear: 0 } : { minYear: years[0], maxYear: years[years.length - 1] };
}

export async function getCurrentDateInfo(isFiscalYear = false): Promise<CurrentDateInfo> {
  const today = new Date();
  const nepaliDate = await convertToNepali(today);
  const { year, month, day } = nepaliDate;
  const weekOfMonth = Math.floor((day - 1) / 7) + 1;

  let quarter: number;
  if (isFiscalYear) {
    if (month >= 4 && month <= 6) quarter = 1;
    else if (month >= 7 && month <= 9) quarter = 2;
    else if (month >= 10 && month <= 12) quarter = 3;
    else quarter = 4;
  } else {
    quarter = Math.floor((month - 1) / 3) + 1;
  }

  const fiscalYear = isFiscalYear && month >= 1 && month <= 3 ? year - 1 : year;
  const localToday = parseDateString(toKathmanduDateString(today));

  return {
    date: nepaliDate,
    adDate: toKathmanduDateString(today),
    year,
    month,
    day,
    monthName: MONTH_NAMES_EN[month - 1],
    nepaliMonthName: MONTH_NAMES_NP[month - 1],
    quarter,
    weekOfMonth,
    fiscalYear,
    dayOfWeek: dayOfWeekName(localToday),
    isWeekend: isWeekend(localToday.getUTCDay()),
  };
}
