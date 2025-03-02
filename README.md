# NepaliCalendarToolkit

This document provides an overview of the `NepaliCalendarConverter` class, detailing its methods, their implementations, and sample responses.

```csharp
dotnet add package NepaliCalendarToolkit 
```
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

### 6. GetHolidaysAndWeekends

**Description:** Retrieves a list of holidays and weekends for a specified year and optional month.

**Implementation:**

```csharp
var holidays = NepaliCalendarConverter.GetHolidaysAndWeekends(2080);
```
Response:
```csharp
List<HolidayInfo> { ... } // Contains holiday information for the year 2080
```
```json
{
  "Holidays": [
    {
      "Date": "2023-09-17",
      "Day": "Sunday",
      "Holiday": "Constitution Day"
    },
    {
      "Date": "2023-10-01",
      "Day": "Sunday",
      "Holiday": "Ghatasthapana"
    },
    {
      "Date": "2023-10-11",
      "Day": "Wednesday",
      "Holiday": "Fulpati"
    },
    {
      "Date": "2023-10-15",
      "Day": "Sunday",
      "Holiday": "Maha Asthami"
    },
    {
      "Date": "2023-10-16",
      "Day": "Monday",
      "Holiday": "Maha Navami"
    },
    {
      "Date": "2023-10-17",
      "Day": "Tuesday",
      "Holiday": "Vijaya Dashami"
    },
    {
      "Date": "2023-10-18",
      "Day": "Wednesday",
      "Holiday": "Ekadashi"
    },
    {
      "Date": "2023-10-19",
      "Day": "Thursday",
      "Holiday": "Dwadashi"
    },
    {
      "Date": "2023-10-20",
      "Day": "Friday",
      "Holiday": "Kojagrat Purnima"
    },
    {
      "Date": "2023-10-21",
      "Day": "Saturday",
      "Holiday": "Tihar"
    },
    {
      "Date": "2023-10-22",
      "Day": "Sunday",
      "Holiday": "Tihar"
    },
    {
      "Date": "2023-10-23",
      "Day": "Monday",
      "Holiday": "Tihar"
    },
    {
      "Date": "2023-10-24",
      "Day": "Tuesday",
      "Holiday": "Tihar"
    },
    {
      "Date": "2023-10-25",
      "Day": "Wednesday",
      "Holiday": "Tihar"
    },
    {
      "Date": "2023-10-26",
      "Day": "Thursday",
      "Holiday": "Tihar"
    }
  ]
}
## Release Notes
### Version main
Released on 
## Release Notes
### Version main
Released on 
## Release Notes
### Version main
Released on 
