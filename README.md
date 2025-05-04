# Nepali Calendar Toolkit

This NuGet package provides a comprehensive toolkit for handling Nepali calendar dates, including Nepali public holidays
and weekends from 2065 BS - 2082 BS.

## Features

- Convert between Nepali (Bikram Sambat) and Gregorian (AD) dates
- Get Nepali public holidays
- Calculate weekends for Nepali dates
- Work with Nepali fiscal years (starts on Shrawan 1st and ends on Ashar end)
- Get date ranges for Nepali months, quarters, and fiscal years
- JSON-based holiday data storage

## Installation

```
Install-Package NepaliCalendarToolkit
```

Or using the .NET CLI:

```
dotnet add package NepaliCalendarToolkit
```

## Usage

### Get Holidays and Weekends

```csharp
// Get all holidays and weekends for a specific year
var holidaysAndWeekends = NepaliCalendarConverter.GetHolidaysAndWeekends(2080);

// Get only holidays for a specific month in a year
var holidays = NepaliCalendarConverter.GetHolidaysAndWeekends(2080, 1, HolidayOrWeekendEnum.Holidays);

// Get only weekends for a specific month in a year
var weekends = NepaliCalendarConverter.GetHolidaysAndWeekends(2080, 1, HolidayOrWeekendEnum.Weekends);
```

### Get Available Date Ranges

```csharp
// Get the year range for which holiday data is available
var (minHolidayYear, maxHolidayYear) = NepaliCalendarConverter.GetAvailableHolidayYearsBS();
// Example output: minHolidayYear: 2065, maxHolidayYear: 2083

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
var nepaliDate = new NepaliDate(2080, 1, 1);
var gregorianDate = NepaliCalendarConverter.ConvertToAD(nepaliDate);

// Convert from Gregorian date to Nepali date
var gregorianDate = new DateTime(2023, 4, 14);
var nepaliDate = NepaliCalendarConverter.ConvertToNepali(gregorianDate);
```

### Fiscal Year Operations

```csharp
// Get date range for a Nepali fiscal year
var fiscalYearRange = NepaliCalendarConverter.GetFiscalYearDateRangeInAD(2080);
// Result: StartDate: 2023-07-17, EndDate: 2024-07-15

// Get holidays for a fiscal year
var fiscalYearHolidays = NepaliCalendarConverter.GetHolidaysAndWeekendsForFiscalYear(2080);
```

## Holiday Data Structure

The toolkit now stores holiday data in JSON files (in the Data/Holidays directory) with the following structure:

```json
[
  {
    "month": 1,
    "day": 11,
    "date": "2008-04-23",
    "name": "Loktantra Diwas"
  }
  // More holidays...
]
```

Each year has its own JSON file named with the BS year (e.g., "2080.json").

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

### 7. GetHolidaysAndWeekends

**Description:** Retrieves a list of holidays and weekends for a specified year and optional month.

**Implementation:**

```csharp
var holidays = NepaliCalendarConverter.GetHolidaysAndWeekends(2080);
```

Response:

```csharp
List<HolidayInfo> { ... } // Contains holiday information for the year 2080
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

### 9. GetHolidaysAndWeekendsForFiscalYear

**Description:** Retrieves a list of holidays and weekends for a specified fiscal year.

**Implementation:**

```csharp
var fiscalYearHolidays = NepaliCalendarConverter.GetHolidaysAndWeekendsForFiscalYear(2080);
```

Response:

```csharp
List<HolidayInfo> { ... } // Contains holiday information for the fiscal year 2080-81
```
