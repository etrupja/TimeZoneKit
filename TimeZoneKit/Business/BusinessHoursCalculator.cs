using TimeZoneKit.Core;
using TimeZoneKit.Models;

namespace TimeZoneKit.Business;

/// <summary>
/// Provides business hours calculations.
/// </summary>
internal static class BusinessHoursCalculator
{
    /// <summary>
    /// Default business hours (9 AM to 5 PM, Monday through Friday).
    /// </summary>
    private static readonly TimeRange DefaultBusinessHours = new TimeRange(9, 17);

    /// <summary>
    /// Checks if the specified DateTime is during standard business hours in the given timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to check (in UTC or unspecified).</param>
    /// <param name="timeZoneId">The timezone ID.</param>
    /// <param name="startHour">Business day start hour (default: 9).</param>
    /// <param name="endHour">Business day end hour (default: 17).</param>
    /// <returns>True if during business hours; otherwise, false.</returns>
    public static bool IsBusinessHour(DateTime dateTime, string timeZoneId, int startHour = 9, int endHour = 17)
    {
        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);

        // Convert to target timezone
        DateTime localTime;
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
        else if (dateTime.Kind == DateTimeKind.Local)
        {
            localTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }
        else
        {
            // Assume UTC for unspecified
            localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        // Check if it's a weekday
        if (localTime.DayOfWeek == DayOfWeek.Saturday || localTime.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        // Check if within business hours
        var hour = localTime.Hour;
        return hour >= startHour && hour < endHour;
    }

    /// <summary>
    /// Finds the next business hour from the specified DateTime in the given timezone.
    /// </summary>
    /// <param name="dateTime">The starting DateTime (in UTC or unspecified).</param>
    /// <param name="timeZoneId">The timezone ID.</param>
    /// <param name="startHour">Business day start hour (default: 9).</param>
    /// <param name="endHour">Business day end hour (default: 17).</param>
    /// <param name="maxDaysToCheck">Maximum days to search forward (default: 7).</param>
    /// <returns>The next business hour DateTime in UTC, or null if none found.</returns>
    public static DateTime? NextBusinessHour(DateTime dateTime, string timeZoneId, int startHour = 9, int endHour = 17, int maxDaysToCheck = 7)
    {
        var timeZone = TimeZoneResolver.GetTimeZoneInfo(timeZoneId);

        // Convert to target timezone
        DateTime localTime;
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
        else if (dateTime.Kind == DateTimeKind.Local)
        {
            localTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }
        else
        {
            localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        var current = localTime;

        for (int i = 0; i < maxDaysToCheck * 24; i++) // Check hour by hour
        {
            // Skip weekends
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                // Check if within business hours
                if (current.Hour >= startHour && current.Hour < endHour)
                {
                    // Convert back to UTC
                    return TimeZoneInfo.ConvertTimeToUtc(current, timeZone);
                }
            }

            // Move to next hour
            current = current.AddHours(1);

            // If we've moved past business hours for today, jump to start of next business day
            if (current.Hour >= endHour || current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
            {
                // Move to next day at start hour
                current = current.Date.AddDays(1).AddHours(startHour);
            }
        }

        return null;
    }

    /// <summary>
    /// Checks if the specified DateTime is during business hours using a BusinessHours configuration.
    /// </summary>
    /// <param name="dateTime">The DateTime to check (should be in UTC).</param>
    /// <param name="businessHours">The business hours configuration.</param>
    /// <returns>True if during business hours; otherwise, false.</returns>
    public static bool IsBusinessHour(DateTime dateTime, BusinessHours businessHours)
    {
        var timeZone = TimeZoneResolver.GetTimeZoneInfo(businessHours.TimeZoneId);

        // Convert to target timezone
        DateTime localTime;
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            localTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }
        else
        {
            localTime = dateTime;
        }

        return businessHours.IsOpen(localTime);
    }
}
