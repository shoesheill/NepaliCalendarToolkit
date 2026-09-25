# Nepali Calendar Toolkit

A .NET library for working with Nepali (Bikram Sambat, BS) dates in calendar and fiscal-year contexts.

![Nepali Calendar Toolkit for .NET overview](https://cdn.nepali.calendar.localhub.dev/NepaliCalendarToolkit/assets/nepali-calendar-toolkit-dotnet-overview.svg)

## Features

- **Get AD date ranges for Nepali months, weeks, month sequences, quarters, calendar years, and fiscal years—without manually handling variable month lengths or fiscal-year boundaries**
- Convert Gregorian (AD) dates to Nepali (BS) dates and back
- Retrieve rich events (festivals, government holidays, observances) by BS year, month, or date
- Retrieve daily details: tithi, paksha, lunar month, weekday, and Nepal Sambat labels
- Configure weekend days; Saturday and Sunday are the defaults
- Get a complete summary of the current date, including BS date, quarter, week of month, fiscal year, weekday, and weekend status
- Check supported calendar years, available months, and whether a year is provisional
- Use with .NET Standard 2.0, .NET Standard 2.1, and .NET 9

## Installation

### .NET CLI

```bash
dotnet add package NepaliCalendarToolkit
```

### Package Manager Console

```powershell
Install-Package NepaliCalendarToolkit
```

## Getting Started

Import the converter and model namespaces:

```csharp
using System;
using NepaliCalendarToolkit.Converters;
using NepaliCalendarToolkit.Models;
```

### Convert Between AD and BS

```csharp
// AD -> BS
NepaliDate nepaliDate = NepaliCalendarConverter.ConvertToNepali(
    new DateTime(2024, 4, 13));

Console.WriteLine(nepaliDate.GetYear);  // 2081
Console.WriteLine(nepaliDate.GetMonth); // 1
Console.WriteLine(nepaliDate.GetDay);   // 1
Console.WriteLine(nepaliDate);          // 2081-01-01

// BS -> AD
DateTime gregorianDate = NepaliCalendarConverter.ConvertToAd(
    new NepaliDate(2081, 1, 1));

Console.WriteLine(gregorianDate.ToString("yyyy-MM-dd")); // 2024-04-13
```

### Get Nepali Months, Quarters, and Fiscal-Year Date Ranges

> **Get production-ready AD start and end dates for any Nepali month, quarter, calendar year, or fiscal year—without manually handling Nepali month lengths or year boundaries.**

Use these methods for common reporting periods in Nepal, including **Nepali months**, **Nepali quarters**, and **Nepali fiscal years**. Every method returns `StartDate` and `EndDate` as `yyyy-MM-dd` strings.

```csharp
// One BS month
var month = NepaliCalendarConverter.GetMonthDateInAd(2081, 1);

// One week within a month
var week = NepaliCalendarConverter.GetWeekDateInAd(2081, 1, 2);

// Months 6 through 8 in one BS year
var months = NepaliCalendarConverter.GetMonthRangeDateInAd(2081, 6, 8);

// A regular-quarter BS year
var quarter = NepaliCalendarConverter.GetQuarterDateRangeInAd(2081, 1);

// A fiscal-year quarter (Shrawan to Ashar)
var fiscalQuarter = NepaliCalendarConverter.GetQuarterDateRangeInAd(
    2080, 1, isFiscalYear: true);

// A complete BS calendar year
var calendarYear = NepaliCalendarConverter.GetYearDateRangeInAd(2081);

// A fiscal year: Shrawan 2080 through the end of Ashar 2081
var fiscalYear = NepaliCalendarConverter.GetYearDateRangeInAd(
    2080, isFiscalYear: true);

Console.WriteLine(fiscalYear.StartDate);
Console.WriteLine(fiscalYear.EndDate);
```

### Get Daily Details and Events

```csharp
// Tithi, paksha, lunar month, weekday, and Nepal Sambat labels for one BS date
var day = NepaliCalendarConverter.GetDayDetails(new NepaliDate(2083, 6, 5));

Console.WriteLine(day.AdDate);           // 2026-09-21
Console.WriteLine(day.TithiNumber);      // 10
Console.WriteLine(day.Paksha);           // शुक्ल
Console.WriteLine(day.LunarMonthName);   // भाद्र
Console.WriteLine(day.NepaliSambatYear); // 1146

// Rich events: festivals, government holidays, observances
var yearEvents = NepaliCalendarConverter.GetEvents(2083);
var monthEvents = NepaliCalendarConverter.GetEvents(2083, month: 5);
var dateEvents = NepaliCalendarConverter.GetEventsForDate(
    new NepaliDate(2083, 5, 12));
```

`CalendarEvent` records carry the AD date, BS month/day (the year is the file name), English/Nepali
names, category, holiday type, government-holiday flag, and Nepal Sambat fields. Provider-specific
identifiers, media URLs and provider bookkeeping are intentionally not stored.

> **Breaking change:** `HolidayInfo`, `HolidayJson`, `HolidayOrWeekendEnum`, and
> `GetHolidaysAndWeekends` were removed. Use `GetEvents` / `GetEventsForDate` for
> events and `GetDayDetails` for tithi and lunar details.

### Configure Weekend Days

The default weekend is Saturday and Sunday. Weekend configuration applies to weekend checks and current-date information.

```csharp
// Saturday only
NepaliCalendarConverter.ConfigureWeekendDays(DayOfWeek.Saturday);

// Friday and Saturday
NepaliCalendarConverter.ConfigureWeekendDays(
    DayOfWeek.Friday,
    DayOfWeek.Saturday);

// Read the current configuration
DayOfWeek[] configuredDays =
    NepaliCalendarConverter.GetConfiguredWeekendDays();
```

### Get Current Date Information

```csharp
var today = NepaliCalendarConverter.GetCurrentDateInfo();

Console.WriteLine(today.Date);        // Current date in BS
Console.WriteLine(today.AdDate);      // Current Gregorian date
Console.WriteLine(today.MonthName);   // For example: Baisakh
Console.WriteLine(today.Quarter);     // 1-4
Console.WriteLine(today.WeekOfMonth); // 1-5
Console.WriteLine(today.DayOfWeek);   // Current day of the week
Console.WriteLine(today.IsWeekend);   // Uses the configured weekend days
```

Use fiscal-year context when quarter and fiscal-year values should follow the Nepali fiscal calendar:

```csharp
var fiscalToday = NepaliCalendarConverter.GetCurrentDateInfo(
    isFiscalYear: true);
```

### Check Supported Coverage

```csharp
var calendarYears = NepaliCalendarConverter.GetAvailableCalendarYearsBs();

int knownMonths = NepaliCalendarConverter.GetKnownMonthsBs(2081);
bool provisional = NepaliCalendarConverter.IsProvisionalBs(2081);

Console.WriteLine($"Calendar years: {calendarYears.MinYear}-{calendarYears.MaxYear}");
Console.WriteLine($"Known months in 2081 BS: {knownMonths}");
Console.WriteLine($"2081 BS is provisional: {provisional}");
```

## Notes

- Nepali months are represented by numbers from `1` (Baisakh) to `12` (Chaitra).
- A Nepali fiscal year starts on Shrawan 1 and ends at the end of Ashadh in the following BS year.
- Calls outside the supported calendar coverage throw an exception.
