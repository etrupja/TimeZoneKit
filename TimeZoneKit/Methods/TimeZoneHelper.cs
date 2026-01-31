using TimeZoneKit.Business;
using TimeZoneKit.Core;
using TimeZoneKit.Geographic;
using TimeZoneKit.Models;

namespace TimeZoneKit.Methods;

/// <summary>
/// Static helper methods for DateTime timezone operations.
/// </summary>
public static class TimeZoneHelper
{
    /// <summary>
    /// Converts a DateTime from one timezone to another.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <param name="toTimeZoneId">Target timezone ID (supports IANA or Windows IDs).</param>
    /// <returns>The converted DateTime in the target timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when toTimeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var utcTime = new DateTime(2025, 1, 28, 15, 0, 0, DateTimeKind.Utc);
    /// var nyTime = DateTimeExtensions.Convert(utcTime, "America/New_York");
    /// </code>
    /// </example>
    public static DateTime Convert(DateTime dateTime, string toTimeZoneId)
    {
        if (toTimeZoneId == null)
        {
            throw new ArgumentNullException(nameof(toTimeZoneId));
        }

        var targetTimeZone = TimeZoneResolver.GetTimeZoneInfo(toTimeZoneId);

        // Convert to UTC first if needed
        DateTime utcTime;
        if (dateTime.Kind == DateTimeKind.Local)
        {
            utcTime = dateTime.ToUniversalTime();
        }
        else if (dateTime.Kind == DateTimeKind.Utc)
        {
            utcTime = dateTime;
        }
        else
        {
            // Unspecified kind - assume UTC
            utcTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, targetTimeZone);
    }

    /// <summary>
    /// Converts a DateTime from a source timezone to a target timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <param name="fromTimeZoneId">Source timezone ID (supports IANA or Windows IDs).</param>
    /// <param name="toTimeZoneId">Target timezone ID (supports IANA or Windows IDs).</param>
    /// <returns>The converted DateTime in the target timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when either timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when any timezone ID is null.</exception>
    /// <example>
    /// <code>
    /// var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
    /// var tokyoTime = DateTimeExtensions.Convert(localTime, "America/New_York", "Asia/Tokyo");
    /// </code>
    /// </example>
    public static DateTime Convert(DateTime dateTime, string fromTimeZoneId, string toTimeZoneId)
    {
        if (fromTimeZoneId == null)
        {
            throw new ArgumentNullException(nameof(fromTimeZoneId));
        }
        if (toTimeZoneId == null)
        {
            throw new ArgumentNullException(nameof(toTimeZoneId));
        }

        var sourceTimeZone = TimeZoneResolver.GetTimeZoneInfo(fromTimeZoneId);
        var targetTimeZone = TimeZoneResolver.GetTimeZoneInfo(toTimeZoneId);

        return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, targetTimeZone);
    }

    /// <summary>
    /// Converts the DateTime to the specified timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <param name="timeZoneId">Target timezone ID (IANA or Windows).</param>
    /// <returns>Converted DateTime in the target timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var utcTime = DateTime.UtcNow;
    /// var nyTime = DateTimeExtensions.ToTimeZone(utcTime, "America/New_York");
    /// </code>
    /// </example>
    public static DateTime ToTimeZone(DateTime dateTime, string timeZoneId)
    {
        return Convert(dateTime, timeZoneId);
    }

    /// <summary>
    /// Converts the DateTime from a source timezone to a target timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <param name="fromTimeZoneId">Source timezone ID (IANA or Windows).</param>
    /// <param name="toTimeZoneId">Target timezone ID (IANA or Windows).</param>
    /// <returns>Converted DateTime in the target timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when either timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when any timezone ID is null.</exception>
    /// <example>
    /// <code>
    /// var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
    /// var tokyoTime = DateTimeExtensions.ToTimeZone(localTime, "America/New_York", "Asia/Tokyo");
    /// </code>
    /// </example>
    public static DateTime ToTimeZone(DateTime dateTime, string fromTimeZoneId, string toTimeZoneId)
    {
        return Convert(dateTime, fromTimeZoneId, toTimeZoneId);
    }

    /// <summary>
    /// Converts a local DateTime to UTC.
    /// </summary>
    /// <param name="localTime">The local DateTime to convert.</param>
    /// <param name="timeZoneId">The timezone ID of the local time (supports IANA or Windows IDs).</param>
    /// <returns>The UTC DateTime.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
    /// var utcTime = DateTimeExtensions.ToUtc(localTime, "America/New_York");
    /// </code>
    /// </example>
    public static DateTime ToUtc(DateTime localTime, string timeZoneId)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);
    }

    /// <summary>
    /// Gets the TimeZoneInfo for the specified timezone identifier.
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier (supports IANA or Windows IDs).</param>
    /// <returns>The TimeZoneInfo instance.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var tzInfo = DateTimeExtensions.GetTimeZoneInfo("America/New_York");
    /// Console.WriteLine(tzInfo.DisplayName);
    /// </code>
    /// </example>
    public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        return TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
    }

    /// <summary>
    /// Converts an IANA timezone ID to a Windows timezone ID.
    /// </summary>
    /// <param name="ianaId">The IANA timezone identifier (e.g., "America/New_York").</param>
    /// <returns>The Windows timezone ID (e.g., "Eastern Standard Time"), or null if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when ianaId is null.</exception>
    /// <example>
    /// <code>
    /// var windowsId = DateTimeExtensions.IanaToWindows("America/New_York");
    /// // Returns: "Eastern Standard Time"
    /// </code>
    /// </example>
    public static string? IanaToWindows(string ianaId)
    {
        if (ianaId == null)
        {
            throw new ArgumentNullException(nameof(ianaId));
        }

        return TimeZoneResolver.IanaToWindows(ianaId);
    }

    /// <summary>
    /// Converts a Windows timezone ID to an IANA timezone ID.
    /// </summary>
    /// <param name="windowsId">The Windows timezone identifier (e.g., "Eastern Standard Time").</param>
    /// <returns>The IANA timezone ID (e.g., "America/New_York"), or null if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when windowsId is null.</exception>
    /// <example>
    /// <code>
    /// var ianaId = DateTimeExtensions.WindowsToIana("Eastern Standard Time");
    /// // Returns: "America/New_York"
    /// </code>
    /// </example>
    public static string? WindowsToIana(string windowsId)
    {
        if (windowsId == null)
        {
            throw new ArgumentNullException(nameof(windowsId));
        }

        return TimeZoneResolver.WindowsToIana(windowsId);
    }

    /// <summary>
    /// Gets the UTC offset for a timezone at a specific date and time.
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier (supports IANA or Windows IDs).</param>
    /// <param name="dateTime">The date and time to check the offset for.</param>
    /// <returns>The UTC offset as a TimeSpan.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var offset = DateTimeExtensions.GetOffsetAt("America/New_York", DateTime.Now);
    /// Console.WriteLine($"Offset: {offset.TotalHours} hours");
    /// </code>
    /// </example>
    public static TimeSpan GetOffsetAt(string timeZoneId, DateTime dateTime)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
        return timeZone.GetUtcOffset(dateTime);
    }

    /// <summary>
    /// Gets the UTC offset for the DateTime in the specified timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to check.</param>
    /// <param name="timeZoneId">The timezone ID (IANA or Windows).</param>
    /// <returns>The UTC offset as a TimeSpan.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var offset = DateTimeExtensions.GetOffset(DateTime.Now, "America/New_York");
    /// Console.WriteLine($"Current offset: {offset.TotalHours} hours");
    /// </code>
    /// </example>
    public static TimeSpan GetOffset(DateTime dateTime, string timeZoneId)
    {
        return GetOffsetAt(timeZoneId, dateTime);
    }

    /// <summary>
    /// Determines whether the specified timezone supports daylight saving time.
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier (supports IANA or Windows IDs).</param>
    /// <returns>True if the timezone supports DST; otherwise, false.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var hasDst = DateTimeExtensions.SupportsDst("America/New_York");
    /// // Returns: true
    /// </code>
    /// </example>
    public static bool SupportsDst(string timeZoneId)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);

        // Check if the timezone has any current or future DST rules
        // This is more accurate than SupportsDaylightSavingTime which returns true
        // for timezones with historical DST rules that are no longer in effect
        var adjustmentRules = timeZone.GetAdjustmentRules();
        if (adjustmentRules.Length == 0)
        {
            return false;
        }

        var today = DateTime.UtcNow.Date;
        foreach (var rule in adjustmentRules)
        {
            // Check if the rule is current or future
            if (rule.DateEnd >= today || rule.DateEnd == DateTime.MaxValue.Date)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified date and time is in daylight saving time.
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier (supports IANA or Windows IDs).</param>
    /// <param name="dateTime">The date and time to check.</param>
    /// <returns>True if the date/time is in DST; otherwise, false.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var isDst = DateTimeExtensions.IsDaylightSavingTime("America/New_York", DateTime.Now);
    /// </code>
    /// </example>
    public static bool IsDaylightSavingTime(string timeZoneId, DateTime dateTime)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
        return timeZone.IsDaylightSavingTime(dateTime);
    }

    /// <summary>
    /// Determines whether the DateTime is in daylight saving time for the specified timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to check.</param>
    /// <param name="timeZoneId">The timezone ID (IANA or Windows).</param>
    /// <returns>True if in DST; otherwise, false.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var isDst = DateTimeExtensions.IsDaylightSavingTime(DateTime.Now, "America/New_York");
    /// </code>
    /// </example>
    public static bool IsDaylightSavingTime(DateTime dateTime, string timeZoneId)
    {
        return IsDaylightSavingTime(timeZoneId, dateTime);
    }

    /// <summary>
    /// Gets a list of common timezones suitable for dropdown lists.
    /// </summary>
    /// <returns>An array of commonly used IANA timezone IDs.</returns>
    /// <example>
    /// <code>
    /// var commonZones = DateTimeExtensions.GetCommonTimezones();
    /// foreach (var zone in commonZones)
    /// {
    ///     Console.WriteLine(zone);
    /// }
    /// </code>
    /// </example>
    public static string[] GetCommonTimezones()
    {
        return TimeZoneMappings.CommonTimezones;
    }

    /// <summary>
    /// Gets the friendly display name for a timezone.
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier (supports IANA or Windows IDs).</param>
    /// <returns>The friendly display name.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var displayName = DateTimeExtensions.GetFriendlyName("America/New_York");
    /// // Returns: "Eastern Time (US &amp; Canada)"
    /// </code>
    /// </example>
    public static string GetFriendlyName(string timeZoneId)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        if (TimeZoneMappings.DisplayNames.TryGetValue(timeZoneId, out var displayName))
        {
            return displayName;
        }

        // Fallback to system display name
        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
        return timeZone.DisplayName;
    }

    /// <summary>
    /// Parses a timezone identifier from various input formats including abbreviations, city names, offsets, and IANA/Windows IDs.
    /// </summary>
    /// <param name="input">The input string to parse (e.g., "EST", "New York", "GMT-5", "America/New_York").</param>
    /// <returns>The TimeZoneInfo for the parsed timezone.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when the timezone cannot be resolved.</exception>
    /// <exception cref="ArgumentException">Thrown when input is null or empty.</exception>
    /// <example>
    /// <code>
    /// var tz1 = DateTimeExtensions.Parse("EST");              // Abbreviation
    /// var tz2 = DateTimeExtensions.Parse("New York");         // City name
    /// var tz3 = DateTimeExtensions.Parse("GMT-5");            // Offset
    /// var tz4 = DateTimeExtensions.Parse("America/New_York"); // IANA ID
    /// </code>
    /// </example>
    public static TimeZoneInfo Parse(string input)
    {
        var timeZoneId = TimeZoneParser.Parse(input);
        return TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
    }

    /// <summary>
    /// Tries to parse a timezone identifier from various input formats.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <param name="timeZoneInfo">The parsed TimeZoneInfo, or null if parsing failed.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    /// <example>
    /// <code>
    /// if (DateTimeExtensions.TryParse("EST", out var tzInfo))
    /// {
    ///     Console.WriteLine($"Parsed: {tzInfo.DisplayName}");
    /// }
    /// </code>
    /// </example>
    public static bool TryParse(string input, out TimeZoneInfo? timeZoneInfo)
    {
        if (TimeZoneParser.TryParse(input, out var timeZoneId) && timeZoneId != null)
        {
            try
            {
                timeZoneInfo = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
                return true;
            }
            catch
            {
                timeZoneInfo = null;
                return false;
            }
        }

        timeZoneInfo = null;
        return false;
    }

    /// <summary>
    /// Searches for timezones matching the given query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>List of matching IANA timezone IDs.</returns>
    /// <example>
    /// <code>
    /// var results = DateTimeExtensions.Search("eastern");
    /// // Returns: ["America/New_York", "America/Detroit", ...]
    /// </code>
    /// </example>
    public static List<string> Search(string query)
    {
        return TimeZoneParser.Search(query);
    }

    /// <summary>
    /// Gets the timezone for a given city name.
    /// </summary>
    /// <param name="cityName">The city name (e.g., "London", "New York", "Tokyo").</param>
    /// <returns>The TimeZoneInfo for the city.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when the city is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when cityName is null or empty.</exception>
    /// <example>
    /// <code>
    /// var londonTz = DateTimeExtensions.FromCity("London");
    /// Console.WriteLine(londonTz.Id); // "Europe/London"
    /// </code>
    /// </example>
    public static TimeZoneInfo FromCity(string cityName)
    {
        var timeZoneId = CityLookup.GetTimeZoneByCity(cityName);
        return TimeZoneResolver.GetTimeZoneInfo(timeZoneId);
    }

    /// <summary>
    /// Gets all timezones for a given country code.
    /// </summary>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code (e.g., "US", "GB", "JP").</param>
    /// <returns>Array of IANA timezone IDs for the country.</returns>
    /// <exception cref="ArgumentException">Thrown when the country code is not found or invalid.</exception>
    /// <example>
    /// <code>
    /// var usTimezones = DateTimeExtensions.GetByCountry("US");
    /// // Returns: ["America/New_York", "America/Chicago", ...]
    /// </code>
    /// </example>
    public static string[] GetByCountry(string countryCode)
    {
        return CountryLookup.GetTimeZonesByCountry(countryCode);
    }

    /// <summary>
    /// Gets timezones that match the specified UTC offset.
    /// </summary>
    /// <param name="offset">The UTC offset to search for.</param>
    /// <returns>List of IANA timezone IDs with the specified base offset.</returns>
    /// <example>
    /// <code>
    /// var zones = DateTimeExtensions.GetByOffset(TimeSpan.FromHours(-5));
    /// // Returns timezones with UTC-5 base offset
    /// </code>
    /// </example>
    public static List<string> GetByOffset(TimeSpan offset)
    {
        var results = new List<string>();

        foreach (var kvp in TimeZoneMappings.Mappings)
        {
            // Remove leading '+' sign if present, as TimeSpan.TryParse doesn't handle it
            var offsetStr = kvp.Value.BaseOffset?.TrimStart('+') ?? string.Empty;

            if (TimeSpan.TryParse(offsetStr, out var baseOffset))
            {
                if (baseOffset == offset)
                {
                    results.Add(kvp.Key);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Checks if the specified DateTime is during standard business hours in the given timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to check (UTC, local, or unspecified).</param>
    /// <param name="timeZoneId">The timezone ID (IANA or Windows).</param>
    /// <param name="startHour">Business day start hour (default: 9).</param>
    /// <param name="endHour">Business day end hour (default: 17).</param>
    /// <returns>True if during business hours (weekdays, 9-5 by default); otherwise, false.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var isOpen = DateTimeExtensions.IsBusinessHour(DateTime.UtcNow, "America/New_York");
    /// </code>
    /// </example>
    public static bool IsBusinessHour(DateTime dateTime, string timeZoneId, int startHour = 9, int endHour = 17)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        return BusinessHoursCalculator.IsBusinessHour(dateTime, timeZoneId, startHour, endHour);
    }

    /// <summary>
    /// Finds the next business hour from the specified DateTime in the given timezone.
    /// </summary>
    /// <param name="dateTime">The starting DateTime (UTC, local, or unspecified).</param>
    /// <param name="timeZoneId">The timezone ID (IANA or Windows).</param>
    /// <param name="startHour">Business day start hour (default: 9).</param>
    /// <param name="endHour">Business day end hour (default: 17).</param>
    /// <returns>The next business hour DateTime in UTC, or null if none found within 7 days.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var nextOpen = DateTimeExtensions.NextBusinessHour(DateTime.UtcNow, "America/New_York");
    /// </code>
    /// </example>
    public static DateTime? NextBusinessHour(DateTime dateTime, string timeZoneId, int startHour = 9, int endHour = 17)
    {
        if (timeZoneId == null)
        {
            throw new ArgumentNullException(nameof(timeZoneId));
        }

        return BusinessHoursCalculator.NextBusinessHour(dateTime, timeZoneId, startHour, endHour);
    }

    /// <summary>
    /// Finds common meeting times across multiple timezones during business hours.
    /// </summary>
    /// <param name="timeZones">Array of timezone IDs to check.</param>
    /// <param name="workingHours">Working hours range (e.g., 9 AM to 5 PM).</param>
    /// <param name="date">Date to check for meeting slots.</param>
    /// <returns>List of time slots where all timezones overlap during business hours.</returns>
    /// <exception cref="ArgumentNullException">Thrown when timeZones or workingHours is null.</exception>
    /// <exception cref="ArgumentException">Thrown when timeZones array is empty.</exception>
    /// <exception cref="TimeZoneNotFoundException">Thrown when any timezone ID is invalid.</exception>
    /// <example>
    /// <code>
    /// var slots = DateTimeExtensions.FindMeetingTime(
    ///     new[] { "America/New_York", "Europe/London", "Asia/Tokyo" },
    ///     new TimeRange(9, 17),
    ///     DateTime.Today
    /// );
    ///
    /// foreach (var slot in slots)
    /// {
    ///     Console.WriteLine($"Meeting possible at {slot.StartTime} - {slot.EndTime}");
    /// }
    /// </code>
    /// </example>
    public static List<MeetingSlot> FindMeetingTime(string[] timeZones, TimeRange workingHours, DateTime date)
    {
        if (timeZones == null)
        {
            throw new ArgumentNullException(nameof(timeZones));
        }

        if (workingHours == null)
        {
            throw new ArgumentNullException(nameof(workingHours));
        }

        return MeetingTimeFinder.FindMeetingTime(timeZones, workingHours, date);
    }

    /// <summary>
    /// Finds common meeting times using custom business hours for each timezone.
    /// </summary>
    /// <param name="businessHours">Array of business hours configurations for each timezone.</param>
    /// <param name="date">Date to check for meeting slots.</param>
    /// <returns>List of time slots where all timezones are within their respective business hours.</returns>
    /// <exception cref="ArgumentNullException">Thrown when businessHours is null.</exception>
    /// <exception cref="ArgumentException">Thrown when businessHours array is empty.</exception>
    /// <example>
    /// <code>
    /// var businessHours = new[]
    /// {
    ///     new BusinessHours("America/New_York", 9, 17),
    ///     new BusinessHours("Asia/Tokyo", 9, 17)
    /// };
    ///
    /// var slots = DateTimeExtensions.FindMeetingTime(businessHours, DateTime.Today);
    /// </code>
    /// </example>
    public static List<MeetingSlot> FindMeetingTime(BusinessHours[] businessHours, DateTime date)
    {
        if (businessHours == null)
        {
            throw new ArgumentNullException(nameof(businessHours));
        }

        return MeetingTimeFinder.FindMeetingTime(businessHours, date);
    }
}
