export type HolidayOrWeekend = "holidays" | "weekends" | "both";

export class NepaliDate {
  constructor(
    public readonly year: number,
    public readonly month: number,
    public readonly day: number,
  ) {}

  toString(): string {
    const mm = String(this.month).padStart(2, "0");
    const dd = String(this.day).padStart(2, "0");
    return `${this.year}-${mm}-${dd}`;
  }
}

/**
 * Provenance for one BS year (Data/month-meta.json).
 *
 * A year can be present without being final: Nepal Patro publishes predictions ahead of the
 * official gazette, and month lengths arrive a month at a time.
 */
export interface YearMeta {
  /** False when any conversion behind the year came back unverified. */
  verified: boolean;
  /** Leading months known (12 = complete). */
  knownMonths: number;
  /** Whether Baisakh 1 of the year is itself verified. */
  yearStartVerified: boolean;
  /** UTC timestamp of the run that last wrote the year. */
  seededAt?: string;
}

export interface HolidayData {
  month: number;
  day: number;
  date?: string;
  name: string;
}

export interface HolidayInfo {
  dayName: string;
  holidayName: string;
  adDate: string; // yyyy-MM-dd
  bsDate: string; // yyyy-MM-dd (BS)
}

export interface CurrentDateInfo {
  date: NepaliDate;
  adDate: string;
  year: number;
  month: number;
  day: number;
  monthName: string;
  nepaliMonthName: string;
  quarter: number;
  weekOfMonth: number;
  fiscalYear: number;
  dayOfWeek: string;
  isWeekend: boolean;
}

export interface DateRange {
  startDate: string;
  endDate: string;
}
