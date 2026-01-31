[![NuGet](https://img.shields.io/nuget/v/TimeZoneKit.svg)](https://www.nuget.org/packages/TimeZoneKit)
[![Publish to NuGet](https://github.com/etrupja/TimeZoneKit/actions/workflows/publish.yml/badge.svg)](https://github.com/etrupja/TimeZoneKit/actions/workflows/publish.yml)

# TimeZoneKit

A simple, intuitive timezone library for .NET that makes working with timezones actually enjoyable.

## Why TimeZoneKit?

- 🎯 **Simple API** - Easier than NodaTime, more powerful than TimeZoneConverter
- 🌍 **Geographic Lookup** - Find timezones by city, coordinates, or country
- 💼 **Business Hours** - Built-in business hours and meeting time calculations
- 🔄 **Smart Parsing** - Parse timezones from abbreviations, names, or offsets
- 🌐 **Localization** - Display names in multiple languages
- ⚡ **Fast** - Cached lookups and optimized for performance
- 📦 **Zero Dependencies** - Only requires System.Text.Json
- 🎨 **Cross-Platform** - Works on Windows, Linux, and macOS

## Installation

```bash
dotnet add package TimeZoneKit
```

Or via NuGet Package Manager:

```
Install-Package TimeZoneKit
```

## Quick Start

```csharp
using TimeZoneKit.Methods;
using TimeZoneKit.Models;

// Simple conversion
var utcTime = DateTime.UtcNow;
var nyTime = TimeZoneHelper.Convert(utcTime, "America/New_York");

// Parse various formats
var tz = TimeZoneHelper.Parse("EST");           // Abbreviation
var tz2 = TimeZoneHelper.Parse("New York");     // City name
var tz3 = TimeZoneHelper.Parse("GMT-5");        // Offset

// Check business hours
var isOpen = TimeZoneHelper.IsBusinessHour(DateTime.Now, "America/New_York");

// Find meeting times across timezones
var slots = TimeZoneHelper.FindMeetingTime(
    new[] { "America/New_York", "Europe/London", "Asia/Tokyo" },
    new TimeRange(9, 17),
    DateTime.Today
);
```

## Features

### 1. Simple Timezone Conversion

```csharp
using TimeZoneKit.Methods;

// Convert from UTC to specific timezone
var utcTime = DateTime.UtcNow;
var nyTime = TimeZoneHelper.Convert(utcTime, "America/New_York");
var londonTime = TimeZoneHelper.Convert(utcTime, "Europe/London");

// Convert to UTC
var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
var utc = TimeZoneHelper.ToUtc(localTime, "America/New_York");

// Convert between timezones
var tokyoTime = TimeZoneHelper.Convert(nyTime, "America/New_York", "Asia/Tokyo");
```

### 2. Smart Timezone Parsing

TimeZoneKit can parse timezones from multiple formats:

```csharp
using TimeZoneKit.Methods;

// Abbreviations
var tz1 = TimeZoneHelper.Parse("EST");
var tz2 = TimeZoneHelper.Parse("PST");
var tz3 = TimeZoneHelper.Parse("GMT");

// City names
var tz4 = TimeZoneHelper.Parse("New York");
var tz5 = TimeZoneHelper.Parse("London");
var tz6 = TimeZoneHelper.Parse("Tokyo");

// Offset strings
var tz7 = TimeZoneHelper.Parse("GMT-5");
var tz8 = TimeZoneHelper.Parse("UTC+9");

// IANA timezone IDs
var tz9 = TimeZoneHelper.Parse("America/New_York");

// Windows timezone IDs
var tz10 = TimeZoneHelper.Parse("Eastern Standard Time");

// Safe parsing
if (TimeZoneHelper.TryParse("EST", out var tzInfo))
{
    Console.WriteLine($"Parsed: {tzInfo.DisplayName}");
}
```

### 3. IANA ↔ Windows Mapping

```csharp
using TimeZoneKit.Methods;

// Convert between IANA and Windows timezone IDs
var windowsId = TimeZoneHelper.IanaToWindows("America/New_York");
// Returns: "Eastern Standard Time"

var ianaId = TimeZoneHelper.WindowsToIana("Eastern Standard Time");
// Returns: "America/New_York"

// Get TimeZoneInfo (works with both formats)
var tzInfo = TimeZoneHelper.GetTimeZoneInfo("America/New_York");
```

### 4. Geographic Lookup

```csharp
using TimeZoneKit.Methods;

// Find timezone by city name
var londonTz = TimeZoneHelper.FromCity("London");
var tokyoTz = TimeZoneHelper.FromCity("Tokyo");

// Get all timezones for a country (ISO 3166-1 alpha-2 code)
var usTimezones = TimeZoneHelper.GetByCountry("US");
// Returns: ["America/New_York", "America/Chicago", "America/Denver", ...]

var jpTimezones = TimeZoneHelper.GetByCountry("JP");
// Returns: ["Asia/Tokyo"]

// Get timezones by UTC offset
var zones = TimeZoneHelper.GetByOffset(TimeSpan.FromHours(-5));
// Returns all timezones with UTC-5 base offset
```

### 5. Search and Discovery

```csharp
using TimeZoneKit.Methods;

// Search for timezones
var results = TimeZoneHelper.Search("eastern");
// Returns: ["America/New_York", "America/Detroit", ...]

// Get common timezones (for dropdown lists)
var common = TimeZoneHelper.GetCommonTimezones();
// Returns: ["America/New_York", "America/Chicago", "Europe/London", ...]

// Get friendly display names
var displayName = TimeZoneHelper.GetFriendlyName("America/New_York");
// Returns: "Eastern Time (US & Canada)"
```

### 6. DST Information

```csharp
using TimeZoneKit.Methods;

// Check if a timezone supports DST
var hasDst = TimeZoneHelper.SupportsDst("America/New_York");
// Returns: true

// Check if a specific date/time is in DST
var isDst = TimeZoneHelper.IsDaylightSavingTime("America/New_York", DateTime.Now);

// Get the UTC offset at a specific time
var offset = TimeZoneHelper.GetOffsetAt("America/New_York", DateTime.Now);
Console.WriteLine($"Current offset: {offset.TotalHours} hours");
```

### 7. Business Hours Support

```csharp
using TimeZoneKit.Methods;
using TimeZoneKit.Models;

// Check if current time is during business hours (9 AM - 5 PM weekdays)
var isOpen = TimeZoneHelper.IsBusinessHour(DateTime.UtcNow, "America/New_York");

// Custom business hours
var isOpen2 = TimeZoneHelper.IsBusinessHour(DateTime.UtcNow, "America/New_York", startHour: 8, endHour: 18);

// Find next business hour
var nextOpen = TimeZoneHelper.NextBusinessHour(DateTime.UtcNow, "America/New_York");
Console.WriteLine($"Next business hour: {nextOpen}");

// Advanced: Custom business hours per day
var businessHours = new BusinessHours("America/New_York", 9, 17)
{
    Saturday = new TimeRange(10, 14), // Open Saturdays 10 AM - 2 PM
    Sunday = null // Closed Sundays
};

var isNowOpen = businessHours.IsOpen(DateTime.Now);
var nextAvailable = businessHours.NextAvailableTime(DateTime.Now);
```

### 8. Meeting Time Finder

Find time slots when all participants are available across different timezones:

```csharp
using TimeZoneKit.Methods;
using TimeZoneKit.Models;

// Find meeting times across multiple timezones
var slots = TimeZoneHelper.FindMeetingTime(
    timeZones: new[] { "America/New_York", "Europe/London", "Asia/Tokyo" },
    workingHours: new TimeRange(9, 17), // 9 AM - 5 PM
    date: DateTime.Today
);

foreach (var slot in slots)
{
    Console.WriteLine($"Meeting window: {slot.StartTime} - {slot.EndTime} UTC");
    Console.WriteLine($"Duration: {slot.Duration.TotalHours} hours");

    // See what time this is in each timezone
    Console.WriteLine($"  NY: {slot.GetStartTimeInZone("America/New_York")}");
    Console.WriteLine($"  London: {slot.GetStartTimeInZone("Europe/London")}");
    Console.WriteLine($"  Tokyo: {slot.GetStartTimeInZone("Asia/Tokyo")}");
}

// Advanced: Custom business hours per timezone
var customHours = new[]
{
    new BusinessHours("America/New_York", 9, 17),
    new BusinessHours("Europe/London", 8, 16),
    new BusinessHours("Asia/Tokyo", 10, 18)
};

var customSlots = TimeZoneHelper.FindMeetingTime(customHours, DateTime.Today);
```

## Supported Timezones

TimeZoneKit includes comprehensive timezone data for **350+ cities**, **60+ countries**, and **110+ timezone mappings** worldwide:

### North America (Complete Coverage)
- **United States**: All time zones including EST, CST, MST, PST, Arizona (no DST), Alaska, Hawaii
- **Canada**: All provinces from Newfoundland (NST) to Pacific, including unique half-hour zones
- **Mexico**: Mexico City, Cancun, Monterrey, Tijuana, and regional variations

### Europe (Comprehensive)
- **Western Europe**: London, Dublin, Lisbon, and Atlantic islands
- **Central Europe**: Paris, Berlin, Rome, Madrid, Amsterdam, Brussels, Zurich, Vienna
- **Northern Europe**: Stockholm, Copenhagen, Oslo, Helsinki
- **Eastern Europe**: Athens, Istanbul, Moscow, Warsaw, Prague, Budapest, Bucharest
- **Russia**: All 11 time zones from Kaliningrad to Kamchatka

### Asia (Full Coverage)
- **Middle East**: Dubai, Riyadh, Tel Aviv, Beirut, Cairo
- **South Asia**: India, Pakistan, Bangladesh, Sri Lanka, Nepal (including unique UTC+5:45)
- **Southeast Asia**: Singapore, Bangkok, Kuala Lumpur, Jakarta, Manila, Ho Chi Minh City
- **East Asia**: Tokyo, Seoul, Shanghai, Beijing, Hong Kong, Taipei

### Pacific & Oceania
- **Australia**: All time zones including Perth, Darwin, Adelaide, Brisbane, Sydney, Melbourne, Hobart
- **New Zealand**: Auckland, Wellington, Christchurch, and Chatham Islands (UTC+12:45)
- **Pacific Islands**: Fiji, Guam, Tonga

### Latin America
- **South America**: Brazil (4 zones), Argentina (12 zones), Chile, Peru, Colombia, Venezuela
- **Central America**: All countries from Panama to Guatemala

### Africa
- **Major Hubs**: Johannesburg, Cairo, Lagos, Nairobi, Casablanca, Addis Ababa

### Special Features
- ✅ Half-hour offsets (India, Newfoundland, Venezuela, etc.)
- ✅ Quarter-hour offsets (Nepal at UTC+5:45)
- ✅ DST and non-DST zones correctly handled
- ✅ Historical timezone changes supported
- ✅ Multi-timezone countries fully mapped

## API Reference

### Core Methods

| Method | Description |
|--------|-------------|
| `Convert(DateTime, string)` | Convert DateTime to target timezone |
| `Convert(DateTime, from, to)` | Convert between two timezones |
| `ToUtc(DateTime, string)` | Convert to UTC |
| `GetTimeZoneInfo(string)` | Get TimeZoneInfo for timezone ID |

### Parsing & Discovery

| Method | Description |
|--------|-------------|
| `Parse(string)` | Smart parse from any format |
| `TryParse(string, out TimeZoneInfo)` | Safe parsing |
| `Search(string)` | Search timezones by keyword |
| `FromCity(string)` | Get timezone by city name |
| `GetByCountry(string)` | Get timezones by country code |
| `GetByOffset(TimeSpan)` | Get timezones by UTC offset |

### Mappings & Display

| Method | Description |
|--------|-------------|
| `IanaToWindows(string)` | Convert IANA to Windows ID |
| `WindowsToIana(string)` | Convert Windows to IANA ID |
| `GetFriendlyName(string)` | Get display name |
| `GetCommonTimezones()` | Get popular timezones |

### DST Information

| Method | Description |
|--------|-------------|
| `SupportsDst(string)` | Check if timezone has DST |
| `IsDaylightSavingTime(string, DateTime)` | Check if in DST |
| `GetOffsetAt(string, DateTime)` | Get UTC offset |

### Business Hours

| Method | Description |
|--------|-------------|
| `IsBusinessHour(DateTime, string)` | Check business hours |
| `NextBusinessHour(DateTime, string)` | Find next business hour |
| `FindMeetingTime(string[], TimeRange, DateTime)` | Cross-timezone meetings |

### Usage Pattern

All methods are accessed via the static `TimeZoneHelper` class:

```csharp
using TimeZoneKit.Methods;

// Conversion
TimeZoneHelper.Convert(dateTime, timeZoneId)
TimeZoneHelper.ToUtc(dateTime, timeZoneId)

// Information
TimeZoneHelper.GetOffset(dateTime, timeZoneId)
TimeZoneHelper.IsDaylightSavingTime(timeZoneId, dateTime)

// Business Hours
TimeZoneHelper.IsBusinessHour(dateTime, timeZoneId)
TimeZoneHelper.NextBusinessHour(dateTime, timeZoneId)
```

## Target Frameworks

- .NET 8.0
- .NET 6.0
- .NET Standard 2.0 (supports .NET Framework 4.6.2+)

## Performance

TimeZoneKit is optimized for performance:

- ✅ **Lazy loading** - Data files loaded only when needed
- ✅ **Thread-safe caching** - TimeZoneInfo instances cached using ConcurrentDictionary
- ✅ **Embedded resources** - No file I/O at runtime
- ✅ **Zero allocations** in hot paths where possible

## Test Coverage

TimeZoneKit is extensively tested with **400+ unit tests** covering:

- ✅ **Parsing**: 100+ tests for abbreviations, city names, IANA IDs, Windows IDs, and offset formats
- ✅ **Geographic Lookup**: 70+ tests for cities, countries, and offset-based searches
- ✅ **Conversions**: 20+ tests for timezone conversions, DST transitions, and round-trip accuracy
- ✅ **DST Handling**: 25+ tests for daylight saving time across multiple regions
- ✅ **Business Hours**: 30+ tests for business hour calculations and meeting time finding
- ✅ **Mapping**: 50+ tests for IANA ↔ Windows conversions and display names
- ✅ **Search**: 15+ tests for search functionality and discovery

All tests passing on .NET 8.0, .NET 6.0, and .NET Standard 2.0.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Acknowledgments

- IANA Timezone Database
- Unicode CLDR for localization data
- Inspired by TimeZoneConverter and NodaTime

## Support

- GitHub Issues: [https://github.com/etrupja/TimeZoneKit/issues](https://github.com/etrupja/TimeZoneKit/issues)
- Documentation: [https://github.com/etrupja/TimeZoneKit](https://github.com/etrupja/TimeZoneKit)

---

Made with ❤️ by Ervis Trupja
