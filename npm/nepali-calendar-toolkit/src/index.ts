export * from "./types";
export * from "./converter";
export * from "./ranges";
export * from "./holidays";
export * from "./currentDate";
export { configure } from "./dataProvider";
export { loadCalendarData, supportedYears, isSupportedYear } from "./calendarData";
export { isWeekend } from "./weekend";

import { loadCalendarData } from "./calendarData";

// Eagerly kick off data loading; the bundled baseline works offline and a CDN
// refresh is merged on top when online.
export const ready = loadCalendarData();

