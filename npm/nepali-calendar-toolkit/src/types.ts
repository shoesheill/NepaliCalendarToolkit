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
