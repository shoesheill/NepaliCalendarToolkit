using System;

public class HolidayInfo
{
    public HolidayInfo(string dayName, string holidayName, DateTime adDate, string bsDate)
    {
        DayName = dayName;
        HolidayName = holidayName;
        AdDate = adDate;
        BsDate = bsDate;
    }

    public string DayName { get; }
    public string HolidayName { get; }
    public DateTime AdDate { get; }
    public string BsDate { get; }
}