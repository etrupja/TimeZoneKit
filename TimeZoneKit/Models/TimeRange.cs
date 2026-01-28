namespace TimeZoneKit.Models;

/// <summary>
/// Represents a time range within a day (e.g., 9 AM to 5 PM).
/// </summary>
public class TimeRange
{
    /// <summary>
    /// Gets or sets the start hour (0-23).
    /// </summary>
    public int StartHour { get; set; }

    /// <summary>
    /// Gets or sets the start minute (0-59).
    /// </summary>
    public int StartMinute { get; set; }

    /// <summary>
    /// Gets or sets the end hour (0-23).
    /// </summary>
    public int EndHour { get; set; }

    /// <summary>
    /// Gets or sets the end minute (0-59).
    /// </summary>
    public int EndMinute { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRange"/> class.
    /// </summary>
    public TimeRange()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRange"/> class with the specified hours.
    /// </summary>
    /// <param name="startHour">The start hour (0-23).</param>
    /// <param name="endHour">The end hour (0-23).</param>
    public TimeRange(int startHour, int endHour)
    {
        if (startHour < 0 || startHour > 23)
            throw new ArgumentOutOfRangeException(nameof(startHour), "Start hour must be between 0 and 23.");
        if (endHour < 0 || endHour > 23)
            throw new ArgumentOutOfRangeException(nameof(endHour), "End hour must be between 0 and 23.");

        StartHour = startHour;
        StartMinute = 0;
        EndHour = endHour;
        EndMinute = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeRange"/> class with the specified hours and minutes.
    /// </summary>
    /// <param name="startHour">The start hour (0-23).</param>
    /// <param name="startMinute">The start minute (0-59).</param>
    /// <param name="endHour">The end hour (0-23).</param>
    /// <param name="endMinute">The end minute (0-59).</param>
    public TimeRange(int startHour, int startMinute, int endHour, int endMinute)
    {
        if (startHour < 0 || startHour > 23)
            throw new ArgumentOutOfRangeException(nameof(startHour), "Start hour must be between 0 and 23.");
        if (startMinute < 0 || startMinute > 59)
            throw new ArgumentOutOfRangeException(nameof(startMinute), "Start minute must be between 0 and 59.");
        if (endHour < 0 || endHour > 23)
            throw new ArgumentOutOfRangeException(nameof(endHour), "End hour must be between 0 and 23.");
        if (endMinute < 0 || endMinute > 59)
            throw new ArgumentOutOfRangeException(nameof(endMinute), "End minute must be between 0 and 59.");

        StartHour = startHour;
        StartMinute = startMinute;
        EndHour = endHour;
        EndMinute = endMinute;
    }

    /// <summary>
    /// Gets the start time as a TimeSpan.
    /// </summary>
    public TimeSpan StartTime => new TimeSpan(StartHour, StartMinute, 0);

    /// <summary>
    /// Gets the end time as a TimeSpan.
    /// </summary>
    public TimeSpan EndTime => new TimeSpan(EndHour, EndMinute, 0);

    /// <summary>
    /// Checks if a given time falls within this time range.
    /// </summary>
    /// <param name="time">The time to check.</param>
    /// <returns>True if the time is within the range; otherwise, false.</returns>
    public bool Contains(TimeSpan time)
    {
        return time >= StartTime && time < EndTime;
    }

    /// <summary>
    /// Checks if a given DateTime falls within this time range (ignoring the date part).
    /// </summary>
    /// <param name="dateTime">The DateTime to check.</param>
    /// <returns>True if the time portion is within the range; otherwise, false.</returns>
    public bool Contains(DateTime dateTime)
    {
        return Contains(dateTime.TimeOfDay);
    }

    /// <summary>
    /// Returns a string representation of this time range.
    /// </summary>
    public override string ToString()
    {
        return $"{StartHour:D2}:{StartMinute:D2} - {EndHour:D2}:{EndMinute:D2}";
    }
}
