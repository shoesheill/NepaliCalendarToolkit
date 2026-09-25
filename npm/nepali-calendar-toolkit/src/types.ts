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

/** One rich event occurrence from Events/<year>.json. */
export interface CalendarEvent {
  adDate: string;
  bsMonth: number;
  bsDay: number;
  nsYear?: number;
  nsMonth?: string;
  nameEn?: string;
  nameNe?: string;
  holidayType?: string;
  category?: string;
  basedOn?: string;
  isGovernmentHoliday: boolean;
  isImportant: boolean;
}

/** Daily details from DayDetails/<year>.json, enriched with display labels. */
export interface DayDetailsData {
  adDate: string;
  bsMonth: number;
  bsDay: number;
  tithi?: number;
  chandrama?: number;
  nsMonth?: string;
  nsYear?: number;
  isVerified: boolean;
}

/** Daily details from DayDetails/<year>.json, enriched with display labels. */
export interface CalendarDayInfo {
  adDate: string;
  bsDate: NepaliDate;
  bsMonthName?: string;
  bsMonthNameEn?: string;
  dayOfWeek: string;
  tithi?: number;
  tithiName?: string;
  tithiNameEn?: string;
  chandrama?: number;
  lunarMonthName?: string;
  lunarMonthNameEn?: string;
  paksha?: string;
  pakshaEn?: string;
  nsMonth?: string;
  nsMonthName?: string;
  nsYear?: number;
  isVerified: boolean;
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
