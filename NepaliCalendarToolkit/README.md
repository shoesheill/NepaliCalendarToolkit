# NepaliCalendarToolkit

This document provides an overview of the `NepaliCalendarConverter` class, detailing its methods, their implementations,
and sample responses.

## Methods

### 1. ConvertToNepali

**Description:** Converts date in AD (`DateTime`) to a Nepali date (`NepaliDate`).

**Implementation:**

```csharp
DateTime dateInAD = new DateTime(2023, 10, 1);
NepaliDate nepaliDate = NepaliCalendarConverter.ConvertToNepali(dateInAD);
```

Response:

```json
{
  "Year": 2080,
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
