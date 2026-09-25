# Nepali Calendar Toolkit

Beyond conversion: This toolkit made easy – configurable weekends, rich events, daily tithi/lunar details, and date ranges for months, quarters, fiscal years, or any custom period.

## Features

- Convert between Nepali (Bikram Sambat) and Gregorian (AD) dates
- Get rich events: festivals, government holidays, and observances
- Get daily details: tithi, paksha, lunar month, weekday, and Nepal Sambat labels
- Calculate weekends(configurable) for Nepali dates
- Work with Nepali fiscal years (starts on Shrawan 1st and ends on Ashar end)
- Get date ranges for Nepali months, quarters, and fiscal years
- JSON-based events and daily-details data storage

## Installation

```
Install-Package NepaliCalendarToolkit
```

Or using the .NET CLI:

```
dotnet add package NepaliCalendarToolkit
```

## Usage

### Get Events and Daily Details

```csharp
// Tithi, paksha, lunar month, weekday, and Nepal Sambat labels
var day = NepaliCalendarConverter.GetDayDetails(new NepaliDate(2083, 6, 5));
Console.WriteLine($"{day.TithiNumber} {day.TithiName}, {day.Paksha} paksha, {day.LunarMonthName}");

// Rich events for a year, a month, or one date
var yearEvents = NepaliCalendarConverter.GetEvents(2083);
var monthEvents = NepaliCalendarConverter.GetEvents(2083, month: 5);
var dateEvents = NepaliCalendarConverter.GetEventsForDate(new NepaliDate(2083, 5, 12));
```

> **Note:** the old `GetHolidaysAndWeekends` API and `HolidayInfo` model were removed;
> use `GetEvents` / `GetEventsForDate` instead. Weekend checks use the configured
> weekend days (see below).

### Get Available Date Ranges

```csharp
// Get the supported Nepali calendar year range
var (minCalendarYear, maxCalendarYear) = NepaliCalendarConverter.GetAvailableCalendarYearsBS();
// Example output: minCalendarYear: 2065, maxCalendarYear: 2083
```

### Configure Weekend Days

```csharp
// Configure just Saturday as a weekend day
NepaliCalendarConverter.ConfigureWeekendDays(DayOfWeek.Saturday);

// Configure both Saturday and Sunday as weekend days (default)
NepaliCalendarConverter.ConfigureWeekendDays(DayOfWeek.Saturday, DayOfWeek.Sunday);

// Configure Friday and Saturday as weekend days
NepaliCalendarConverter.ConfigureWeekendDays(DayOfWeek.Friday, DayOfWeek.Saturday);

// Get the currently configured weekend days
DayOfWeek[] weekendDays = NepaliCalendarConverter.GetConfiguredWeekendDays();
```

### Date Conversion

```csharp
// Convert from Nepali date to Gregorian date
var nepaliDate = new NepaliDate(2082, 1, 1);
var gregorianDate = NepaliCalendarConverter.ConvertToAD(nepaliDate);

// Convert from Gregorian date to Nepali date
var gregorianDate = new DateTime(2025, 4, 14);
var nepaliDate = NepaliCalendarConverter.ConvertToNepali(gregorianDate);
```

### Fiscal Year Operations

```csharp
// Get date range for a Nepali fiscal year
var fiscalYearRange = NepaliCalendarConverter.GetFiscalYearDateRangeInAD(2080);
// Result: StartDate: 2023-07-17, EndDate: 2024-07-15
```

## Data Source

The toolkit loads its data from the [NepaliCalendarToolkit](https://github.com/shoesheill/NepaliCalendarToolkit) repository through its CDN mirror at `https://cdn.nepali.calendar.localhub.dev/` (the mirror serves the repository root; calendar data lives under `Data/`), and stays fully functional offline. This includes:

- Event data under `Data/Events/{year}.json` and daily details under `Data/DayDetails/{year}.json`
- Month lengths data for Nepali calendar calculations
- Year start dates for Nepali calendar

### How data is resolved

Data is resolved in three layers, so it is always fresh when online but never breaks offline:

1. **Live CDN** – the source of truth. Whenever the toolkit is online it fetches the latest data from the
   configured base URL. The project mirror is `https://cdn.nepali.calendar.localhub.dev/`, which serves
   the repository root: the C# provider expects the base URL of the data folder itself
   (`https://cdn.nepali.calendar.localhub.dev/Data/`), while the npm package appends `Data/` for you.
   **Newly added data is picked up automatically without a library version bump.**
2. **Persistent disk cache** – every successful fetch is stored under
   `%LOCALAPPDATA%\NepaliCalendarToolkit\Cache\`. If the device later goes offline, the most recently
   fetched data is used, so the calendar, events, weeks and date ranges all keep working.
3. **Bundled baseline** – the repository-root `Data/` folder (month lengths, year starts, events and
   daily details) is embedded into the NuGet package at build time, so even a first run with no network
   and no cache still works. New files placed there by the seeder are picked up on the next build.

The CDN is consulted at most once every **12 hours** per data file; within that window a cached copy is
served for speed and offline reliability.

### Configuring the data source

The CDN URL is **not hard-coded**. Set it at deployment time in one of two ways:

```csharp
// From appsettings (read your config, then call Configure once at startup):
// The base URL must be the folder that contains month-lengths.json (the Data folder).
NepaliCalendarToolkit.Helpers.DataProvider.Configure(
    baseUrl: "https://cdn.nepali.calendar.localhub.dev/Data/",
    cacheTtlHours: 12);
```

Or set the `DATA_URL` environment variable, which is read automatically:

```
set DATA_URL=https://cdn.nepali.calendar.localhub.dev/Data/
```

If no URL is configured and there is no cache, the embedded baseline is used.



## Event and Daily Data Structure

Events live in `Data/Events/{year}.json`, one file per BS year:

```json
[
  {
    "adDate": "2026-08-28",
    "bsMonth": 5,
    "bsDay": 12,
    "nsYear": 1146,
    "nsMonth": "19.0",
    "nameEn": "Rakshya Bandhan",
    "nameNe": "रक्षाबन्धन",
    "holidayType": "Government Holiday",
    "category": "national",
    "basedOn": "NS",
    "isGovernmentHoliday": true,
    "isImportant": true
  }
]
```

Daily details live in `Data/DayDetails/{year}.json` with one record per day
(`adDate`, `bsMonth`, `bsDay`, `tithi`, `chandrama`, `nsMonth`, `nsYear`, `isVerified`).
Records stay deliberately lean: the BS year is the file name, and provider-specific
identifiers, media URLs and provider bookkeeping (`bsDate`, `remarks`, `updatedAt`)
are not stored.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Methods

### 1. ConvertToNepali

**Description:** Converts date in AD (`DateTime`) to a Nepali date (`NepaliDate`).

**Implementation:**

```csharp
DateTime dateInAD = new DateTime(2024, 10, 1);
NepaliDate nepaliDate = NepaliCalendarConverter.ConvertToNepali(dateInAD);
```

Response:

```json
{
  "Year": 2081,
  "Month": 6,
  "Day": 15
}
```

### 2. ConvertToAD

**Description:** Converts a Nepali date (`NepaliDate`) to a Gregorian date (`DateTime`).

**Implementation:**

```csharp
csharp
NepaliDate nepaliDate = new NepaliDate { Year = 2080, Month = 6, Day = 15 };
DateTime dateInAD = NepaliCalendarConverter.ConvertToAD(nepaliDate);
```

2023-10-01 00:00:00

### 3. GetStartAndEndDateInAD

**Description:** Retrieves the start and end dates in AD for a given Nepali year and month.

**Implementation:**

```csharp
var dateRange = NepaliCalendarConverter.GetStartAndEndDateInAD(2080, 6);
```

Response:

```json
{
  "StartDate": "2023-09-17",
  "EndDate": "2023-10-16"
}
```

### 4. GetQuarterDateRangeInAD

**Description:** Retrieves the start and end dates in AD for a given Nepali year and quarter.

**Implementation:**

```csharp
var quarterRange = NepaliCalendarConverter.GetQuarterDateRangeInAD(2080, 1);
```

Response:

```json
{
  "StartDate": "2023-07-01",
  "EndDate": "2023-09-30"
}
```

### 5. GetMonthRangeDateInAD

**Description:** Retrieves the start and end dates in AD for a specified range of Nepali months.

**Implementation:**

```csharp
var monthRange = NepaliCalendarConverter.GetMonthRangeDateInAD(2080, 6, 8);
```

Response:

```json
{
  "StartDate": "2023-09-17",
  "EndDate": "2023-11-15"
}
```

### 6. GetWeekDateInAd

**Description:** Retrieves the start and end dates in AD for a specific week within a Nepali month.

**Implementation:**

```csharp
// Get the date range for the 2nd week of Asoj month, 2080 BS
var weekRange = NepaliCalendarConverter.GetWeekDateInAd(2080, 6, 2);
```

Response:

```json
{
  "StartDate": "2023-09-24",
  "EndDate": "2023-09-30"
}
```

### 7. GetEvents

**Description:** Retrieves rich event occurrences (festivals, government holidays, observances) for a BS year, optionally filtered by month or to government holidays only.

**Implementation:**

```csharp
var events = NepaliCalendarConverter.GetEvents(2080);
var bhadraEvents = NepaliCalendarConverter.GetEvents(2080, month: 5);
```

Response:

```csharp
List<CalendarEvent> { ... } // AD/BS dates, names, category, holiday type, Nepal Sambat fields
```

### 8. GetFiscalYearDateRangeInAD

**Description:** Retrieves the start and end dates in AD for a given Nepali fiscal year.

**Implementation:**

```csharp
var fiscalYearRange = NepaliCalendarConverter.GetFiscalYearDateRangeInAD(2080);
```

Response:

```json
{
  "StartDate": "2023-07-17",
  "EndDate": "2024-07-15"
}
```

### 9. GetDayDetails

**Description:** Retrieves daily date-conversion details for a BS date: tithi, chandrama (lunar day number), paksha, lunar month, weekday, and Nepal Sambat labels.

**Implementation:**

```csharp
var day = NepaliCalendarConverter.GetDayDetails(new NepaliDate(2083, 6, 5));
```

Response:

```csharp
CalendarDayInfo { ... } // tithi 10 (दशमी), paksha शुक्ल, lunar month भाद्र, NS year 1146
```
