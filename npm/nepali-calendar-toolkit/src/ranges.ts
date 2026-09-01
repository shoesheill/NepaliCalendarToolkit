import { isSupportedYear, monthLengths } from "./calendarData";
import { convertToAd, isValidNepaliMonth, supportedYearsError } from "./converter";
import { DateRange, NepaliDate } from "./types";

export async function getMonthDateInAd(yearBs: number, monthBs: number, isFiscalYear = false): Promise<DateRange> {
  if (monthBs < 1 || monthBs > 12) throw new Error("Month must be between 1 and 12");
  if (isFiscalYear) yearBs = monthBs >= 4 ? yearBs : yearBs + 1;
  if (!isValidNepaliMonth(yearBs, monthBs)) throw new Error("Invalid year or month");

  const startDate = await convertToAd(new NepaliDate(yearBs, monthBs, 1));
  const endDate = await convertToAd(new NepaliDate(yearBs, monthBs, monthLengths[yearBs][monthBs - 1]));
  return { startDate, endDate };
}

export async function getWeekDateInAd(yearBs: number, monthBs: number, weekNumber: number, isFiscalYear = false): Promise<DateRange> {
  if (monthBs < 1 || monthBs > 12) throw new Error("Month must be between 1 and 12");
  if (weekNumber < 1 || weekNumber > 5) throw new Error("Week number must be between 1 and 5");
  if (isFiscalYear) yearBs = monthBs >= 4 ? yearBs : yearBs + 1;
  if (!isValidNepaliMonth(yearBs, monthBs)) throw new Error("Invalid year or month");

  const monthLength = monthLengths[yearBs][monthBs - 1];
  const firstDayOfWeek = (weekNumber - 1) * 7 + 1;
  if (firstDayOfWeek > monthLength) return { startDate: "", endDate: "" };

  const lastDayOfWeek = Math.min(firstDayOfWeek + 6, monthLength);
  const startDate = await convertToAd(new NepaliDate(yearBs, monthBs, firstDayOfWeek));
  const endDate = await convertToAd(new NepaliDate(yearBs, monthBs, lastDayOfWeek));
  return { startDate, endDate };
}

export async function getMonthRangeDateInAd(yearBs: number, startMonth: number, endMonth: number, isFiscalYear = false): Promise<DateRange> {
  if (startMonth < 1 || startMonth > 12 || endMonth < 1 || endMonth > 12) {
    throw new Error("Month must be between 1 and 12");
  }
  if (startMonth > endMonth) throw new Error("Start month cannot be greater than end month");

  let startYearBs = yearBs;
  let endYearBs = yearBs;
  if (isFiscalYear) {
    startYearBs = startMonth >= 4 ? yearBs : yearBs + 1;
    endYearBs = endMonth >= 4 ? yearBs : yearBs + 1;
    if ((startMonth < 4 && endMonth >= 4) || (startMonth >= 4 && endMonth < 4 && endMonth > 0)) {
      throw new Error("Month range must be within the same fiscal year");
    }
  }

  if (!isValidNepaliMonth(startYearBs, startMonth) || !isValidNepaliMonth(endYearBs, endMonth)) {
    throw new Error("Invalid year or month");
  }

  const startDate = await convertToAd(new NepaliDate(startYearBs, startMonth, 1));
  const endDate = await convertToAd(new NepaliDate(endYearBs, endMonth, monthLengths[endYearBs][endMonth - 1]));
  return { startDate, endDate };
}

export async function getQuarterDateRangeInAd(yearBs: number, quarter: number, isFiscalYear = false): Promise<DateRange> {
  if (quarter < 1 || quarter > 4) throw new Error("Invalid quarter");

  let startMonth: number;
  let endMonth: number;
  let calculatedYearBs = yearBs;

  if (isFiscalYear) {
    switch (quarter) {
      case 1: startMonth = 4; endMonth = 6; break;
      case 2: startMonth = 7; endMonth = 9; break;
      case 3: startMonth = 10; endMonth = 12; break;
      default: startMonth = 1; endMonth = 3; calculatedYearBs = yearBs + 1; break;
    }
  } else {
    startMonth = (quarter - 1) * 3 + 1;
    endMonth = startMonth + 2;
  }

  if (!isValidNepaliMonth(calculatedYearBs, startMonth)) throw supportedYearsError();

  const startDate = await convertToAd(new NepaliDate(calculatedYearBs, startMonth, 1));
  const endDate = await convertToAd(
    new NepaliDate(calculatedYearBs, endMonth, monthLengths[calculatedYearBs][endMonth - 1]),
  );
  return { startDate, endDate };
}

export async function getYearDateRangeInAd(yearBs: number, isFiscalYear = false): Promise<DateRange> {
  if (isFiscalYear) {
    if (!isSupportedYear(yearBs) || !isSupportedYear(yearBs + 1)) throw supportedYearsError();
    const startDate = await convertToAd(new NepaliDate(yearBs, 4, 1));
    const endDate = await convertToAd(new NepaliDate(yearBs + 1, 3, monthLengths[yearBs + 1][2]));
    return { startDate, endDate };
  }

  if (!isSupportedYear(yearBs)) throw supportedYearsError();
  const startDate = await convertToAd(new NepaliDate(yearBs, 1, 1));
  const endDate = await convertToAd(new NepaliDate(yearBs, 12, monthLengths[yearBs][11]));
  return { startDate, endDate };
}
