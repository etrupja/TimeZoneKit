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
using TimeZoneKit;
using TimeZoneKit.Extensions;

// Simple conversion
var nyTime = DateTime.UtcNow.ToTimeZone("America/New_York");

// Parse various formats
var tz = TimeZoneKit.Parse("EST");           // Abbreviation
var tz2 = TimeZoneKit.Parse("New York");     // City name
var tz3 = TimeZoneKit.Parse("GMT-5");        // Offset

// Check business hours
var isOpen = DateTime.Now.IsBusinessHour("America/New_York");

// Find meeting times across timezones
var slots = TimeZoneKit.FindMeetingTime(
    new[] { "America/New_York", "Europe/London", "Asia/Tokyo" },
    new TimeRange(9, 17),
    DateTime.Today
);
```

## Features

### 1. Simple Timezone Conversion

```csharp
// Extension methods for DateTime
var utcTime = DateTime.UtcNow;
var nyTime = utcTime.ToTimeZone("America/New_York");
var londonTime = utcTime.ToTimeZone("Europe/London");

// Static methods
var converted = TimeZoneKit.Convert(dateTime, "America/New_York");
var utc = TimeZoneKit.ToUtc(localTime, "EST");

// Convert between timezones
var tokyoTime = nyTime.ToTimeZone("America/New_York", "Asia/Tokyo");
```

### 2. Smart Timezone Parsing

TimeZoneKit can parse timezones from multiple formats:

```csharp
// Abbreviations
var tz1 = TimeZoneKit.Parse("EST");
var tz2 = TimeZoneKit.Parse("PST");
var tz3 = TimeZoneKit.Parse("GMT");

// City names
var tz4 = TimeZoneKit.Parse("New York");
var tz5 = TimeZoneKit.Parse("London");
var tz6 = TimeZoneKit.Parse("Tokyo");

// Offset strings
var tz7 = TimeZoneKit.Parse("GMT-5");
var tz8 = TimeZoneKit.Parse("UTC+9");

// IANA timezone IDs
var tz9 = TimeZoneKit.Parse("America/New_York");

// Windows timezone IDs
var tz10 = TimeZoneKit.Parse("Eastern Standard Time");

// Safe parsing
if (TimeZoneKit.TryParse("EST", out var tzInfo))
{
    Console.WriteLine($"Parsed: {tzInfo.DisplayName}");
}
```

### 3. IANA ↔ Windows Mapping

```csharp
// Convert between IANA and Windows timezone IDs
var windowsId = TimeZoneKit.IanaToWindows("America/New_York");
// Returns: "Eastern Standard Time"

var ianaId = TimeZoneKit.WindowsToIana("Eastern Standard Time");
// Returns: "America/New_York"

// Get TimeZoneInfo (works with both formats)
var tzInfo = TimeZoneKit.GetTimeZoneInfo("America/New_York");
```

### 4. Geographic Lookup

```csharp
// Find timezone by city name
var londonTz = TimeZoneKit.FromCity("London");
var tokyoTz = TimeZoneKit.FromCity("Tokyo");

// Get all timezones for a country (ISO 3166-1 alpha-2 code)
var usTimezones = TimeZoneKit.GetByCountry("US");
// Returns: ["America/New_York", "America/Chicago", "America/Denver", ...]

var jpTimezones = TimeZoneKit.GetByCountry("JP");
// Returns: ["Asia/Tokyo"]

// Get timezones by UTC offset
var zones = TimeZoneKit.GetByOffset(TimeSpan.FromHours(-5));
// Returns all timezones with UTC-5 base offset
```

### 5. Search and Discovery

```csharp
// Search for timezones
var results = TimeZoneKit.Search("eastern");
// Returns: ["America/New_York", "America/Detroit", ...]

// Get common timezones (for dropdown lists)
var common = TimeZoneKit.GetCommonTimezones();
// Returns: ["America/New_York", "America/Chicago", "Europe/London", ...]

// Get friendly display names
var displayName = TimeZoneKit.GetFriendlyName("America/New_York");
// Returns: "Eastern Time (US & Canada)"
```

### 6. DST Information

```csharp
// Check if a timezone supports DST
var hasDst = TimeZoneKit.SupportsDst("America/New_York");
// Returns: true

// Check if a specific date/time is in DST
var isDst = TimeZoneKit.IsDaylightSavingTime("America/New_York", DateTime.Now);

// Get the UTC offset at a specific time
var offset = TimeZoneKit.GetOffsetAt("America/New_York", DateTime.Now);
Console.WriteLine($"Current offset: {offset.TotalHours} hours");

// Extension method
var isDst2 = DateTime.Now.IsDaylightSavingTime("America/New_York");
```

### 7. Business Hours Support

```csharp
// Check if current time is during business hours (9 AM - 5 PM weekdays)
var isOpen = DateTime.UtcNow.IsBusinessHour("America/New_York");

// Custom business hours
var isOpen2 = DateTime.UtcNow.IsBusinessHour("America/New_York", startHour: 8, endHour: 18);

// Find next business hour
var nextOpen = DateTime.UtcNow.NextBusinessHour("America/New_York");
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
// Find meeting times across multiple timezones
var slots = TimeZoneKit.FindMeetingTime(
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

var customSlots = TimeZoneKit.FindMeetingTime(customHours, DateTime.Today);
```

## Supported Timezones

TimeZoneKit includes comprehensive timezone data for:

### United States
- America/New_York (Eastern)
- America/Chicago (Central)
- America/Denver (Mountain)
- America/Los_Angeles (Pacific)
- America/Anchorage (Alaska)
- Pacific/Honolulu (Hawaii)

### Europe
- Europe/London (GMT/BST)
- Europe/Paris (CET/CEST)
- Europe/Berlin
- Europe/Madrid
- Europe/Rome
- Europe/Amsterdam

### Asia
- Asia/Tokyo (Japan)
- Asia/Shanghai (China)
- Asia/Hong_Kong
- Asia/Singapore
- Asia/Dubai (Gulf)
- Asia/Kolkata (India)

### Other Major Regions
- Australia/Sydney
- Australia/Melbourne
- Pacific/Auckland (New Zealand)
- America/Sao_Paulo (Brazil)
- Africa/Johannesburg (South Africa)

Plus 80+ major cities and 25+ countries supported.

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

### Extension Methods

All core methods are available as DateTime extension methods:

```csharp
dateTime.ToTimeZone(timeZoneId)
dateTime.ToUtc(timeZoneId)
dateTime.GetOffset(timeZoneId)
dateTime.IsDaylightSavingTime(timeZoneId)
dateTime.IsBusinessHour(timeZoneId)
dateTime.NextBusinessHour(timeZoneId)
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

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Acknowledgments

- IANA Timezone Database
- Unicode CLDR for localization data
- Inspired by TimeZoneConverter and NodaTime

## Support

- GitHub Issues: [https://github.com/ervistrupja/TimeZoneKit/issues](https://github.com/ervistrupja/TimeZoneKit/issues)
- Documentation: [https://github.com/ervistrupja/TimeZoneKit](https://github.com/ervistrupja/TimeZoneKit)

---

Made with ❤️ by Ervis Trupja
