namespace TimeZoneKit.Models;

/// <summary>
/// Represents business hours configuration for a specific timezone.
/// </summary>
public class BusinessHours
{
    /// <summary>
    /// Gets or sets the timezone ID for these business hours.
    /// </summary>
    public string TimeZoneId { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Monday.
    /// </summary>
    public TimeRange? Monday { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Tuesday.
    /// </summary>
    public TimeRange? Tuesday { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Wednesday.
    /// </summary>
    public TimeRange? Wednesday { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Thursday.
    /// </summary>
    public TimeRange? Thursday { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Friday.
    /// </summary>
    public TimeRange? Friday { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Saturday.
    /// </summary>
    public TimeRange? Saturday { get; set; }

    /// <summary>
    /// Gets or sets the business hours for Sunday.
    /// </summary>
    public TimeRange? Sunday { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessHours"/> class.
    /// </summary>
    /// <param name="timeZoneId">The timezone ID for these business hours.</param>
    public BusinessHours(string timeZoneId)
    {
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessHours"/> class with standard weekday hours.
    /// </summary>
    /// <param name="timeZoneId">The timezone ID for these business hours.</param>
    /// <param name="startHour">The start hour for weekdays.</param>
    /// <param name="endHour">The end hour for weekdays.</param>
    public BusinessHours(string timeZoneId, int startHour, int endHour)
    {
        TimeZoneId = timeZoneId ?? throw new ArgumentNullException(nameof(timeZoneId));
        var range = new TimeRange(startHour, endHour);
        Monday = range;
        Tuesday = range;
        Wednesday = range;
        Thursday = range;
        Friday = range;
    }

    /// <summary>
    /// Gets the business hours for a specific day of the week.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>The TimeRange for that day, or null if not a business day.</returns>
    public TimeRange? GetHoursForDay(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            DayOfWeek.Sunday => Sunday,
            _ => null
        };
    }

    /// <summary>
    /// Checks if the specified DateTime is during business hours.
    /// </summary>
    /// <param name="dateTime">The DateTime to check (assumed to be in the timezone specified by TimeZoneId).</param>
    /// <returns>True if during business hours; otherwise, false.</returns>
    public bool IsOpen(DateTime dateTime)
    {
        var hoursForDay = GetHoursForDay(dateTime.DayOfWeek);
        if (hoursForDay == null)
        {
            return false;
        }

        return hoursForDay.Contains(dateTime.TimeOfDay);
    }

    /// <summary>
    /// Finds the next available business hour from the specified DateTime.
    /// </summary>
    /// <param name="dateTime">The starting DateTime (assumed to be in the timezone specified by TimeZoneId).</param>
    /// <param name="maxDaysToCheck">Maximum number of days to check forward (default: 7).</param>
    /// <returns>The next available business DateTime, or null if none found within maxDaysToCheck.</returns>
    public DateTime? NextAvailableTime(DateTime dateTime, int maxDaysToCheck = 7)
    {
        var current = dateTime;

        for (int i = 0; i < maxDaysToCheck; i++)
        {
            var hoursForDay = GetHoursForDay(current.DayOfWeek);
            if (hoursForDay != null)
            {
                // If we're on the same day and before start time, return start time
                if (i == 0 && current.TimeOfDay < hoursForDay.StartTime)
                {
                    return new DateTime(current.Year, current.Month, current.Day,
                        hoursForDay.StartHour, hoursForDay.StartMinute, 0);
                }
                // If we're on a different day or past business hours, return start of business on this day
                else if (i > 0)
                {
                    return new DateTime(current.Year, current.Month, current.Day,
                        hoursForDay.StartHour, hoursForDay.StartMinute, 0);
                }
            }

            // Move to next day
            current = current.Date.AddDays(1);
        }

        return null;
    }
}
