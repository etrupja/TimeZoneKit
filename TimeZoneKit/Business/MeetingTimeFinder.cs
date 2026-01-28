using TimeZoneKit.Core;
using TimeZoneKit.Models;

namespace TimeZoneKit.Business;

/// <summary>
/// Finds suitable meeting times across multiple timezones.
/// </summary>
internal static class MeetingTimeFinder
{
    /// <summary>
    /// Finds meeting times where all specified timezones overlap during business hours.
    /// </summary>
    /// <param name="timeZones">Array of timezone IDs to consider.</param>
    /// <param name="workingHours">The working hours range (e.g., 9 AM to 5 PM).</param>
    /// <param name="date">The date to check for meeting slots.</param>
    /// <returns>List of meeting slots where all timezones are within business hours.</returns>
    public static List<MeetingSlot> FindMeetingTime(string[] timeZones, TimeRange workingHours, DateTime date)
    {
        if (timeZones == null || timeZones.Length == 0)
        {
            throw new ArgumentException("At least one timezone must be provided.", nameof(timeZones));
        }

        if (workingHours == null)
        {
            throw new ArgumentNullException(nameof(workingHours));
        }

        var slots = new List<MeetingSlot>();

        // Get TimeZoneInfo for all timezones
        var timeZoneInfos = timeZones.Select(tz => TimeZoneResolver.GetTimeZoneInfo(tz)).ToArray();

        // Check each hour of the day
        for (int hour = 0; hour < 24; hour++)
        {
            var utcTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0, DateTimeKind.Utc);
            bool allInBusinessHours = true;

            // Check if this hour is within business hours for all timezones
            foreach (var timeZone in timeZoneInfos)
            {
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);

                // Skip weekends
                if (localTime.DayOfWeek == DayOfWeek.Saturday || localTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    allInBusinessHours = false;
                    break;
                }

                // Check if within working hours
                if (!workingHours.Contains(localTime.TimeOfDay))
                {
                    allInBusinessHours = false;
                    break;
                }
            }

            // If all timezones are in business hours, add this slot
            if (allInBusinessHours)
            {
                var endTime = utcTime.AddHours(1);
                slots.Add(new MeetingSlot(utcTime, endTime));
            }
        }

        // Merge consecutive slots
        return MergeConsecutiveSlots(slots);
    }

    /// <summary>
    /// Finds meeting times using custom business hours for each timezone.
    /// </summary>
    /// <param name="businessHours">Array of business hours configurations for each timezone.</param>
    /// <param name="date">The date to check for meeting slots.</param>
    /// <returns>List of meeting slots where all timezones are within their respective business hours.</returns>
    public static List<MeetingSlot> FindMeetingTime(BusinessHours[] businessHours, DateTime date)
    {
        if (businessHours == null || businessHours.Length == 0)
        {
            throw new ArgumentException("At least one business hours configuration must be provided.", nameof(businessHours));
        }

        var slots = new List<MeetingSlot>();

        // Check each hour of the day
        for (int hour = 0; hour < 24; hour++)
        {
            var utcTime = new DateTime(date.Year, date.Month, date.Day, hour, 0, 0, DateTimeKind.Utc);
            bool allInBusinessHours = true;

            // Check if this hour is within business hours for all configurations
            foreach (var bh in businessHours)
            {
                if (!BusinessHoursCalculator.IsBusinessHour(utcTime, bh))
                {
                    allInBusinessHours = false;
                    break;
                }
            }

            // If all are in business hours, add this slot
            if (allInBusinessHours)
            {
                var endTime = utcTime.AddHours(1);
                slots.Add(new MeetingSlot(utcTime, endTime));
            }
        }

        // Merge consecutive slots
        return MergeConsecutiveSlots(slots);
    }

    private static List<MeetingSlot> MergeConsecutiveSlots(List<MeetingSlot> slots)
    {
        if (slots.Count == 0)
        {
            return slots;
        }

        var merged = new List<MeetingSlot>();
        var current = slots[0];

        for (int i = 1; i < slots.Count; i++)
        {
            if (slots[i].StartTime == current.EndTime)
            {
                // Extend the current slot
                current = new MeetingSlot(current.StartTime, slots[i].EndTime);
            }
            else
            {
                // Add the current slot and start a new one
                merged.Add(current);
                current = slots[i];
            }
        }

        // Add the last slot
        merged.Add(current);

        return merged;
    }
}
