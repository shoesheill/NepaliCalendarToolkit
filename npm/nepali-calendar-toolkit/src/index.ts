export * from "./types";
export * from "./converter";
export * from "./ranges";
export * from "./holidays";
export * from "./currentDate";
export { configure } from "./dataProvider";
export {
  loadCalendarData,
  supportedYears,
  isSupportedYear,
  isSupportedMonth,
  knownMonths,
  completeYears,
  isProvisional,
  getYearInfo,
} from "./calendarData";
export { isWeekend } from "./weekend";
export {
  buildMonthGrid,
  daysInBsMonth,
  monthNames,
  formatBsLong,
  todayBsKey,
  sameBsDay,
  isAdKey,
  type BsDayCell,
  type BsMonthGrid,
} from "./datepicker/calendar";
export {
  BS_WEEKDAY_LABELS,
  NEPALI_DIGITS,
  toPickerDigits,
  normalizeBsKey,
  type BsDatePickerOptions,
  type BsDateSelection,
  type BsDateString,
  type BsLocale,
  type BsWeekdayLabels,
} from "./datepicker/types";

import { loadCalendarData } from "./calendarData";

// Eagerly kick off data loading; the bundled baseline works offline and a CDN
// refresh is merged on top when online.
export const ready = loadCalendarData();

