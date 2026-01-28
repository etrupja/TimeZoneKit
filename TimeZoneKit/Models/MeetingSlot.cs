namespace TimeZoneKit.Models;

/// <summary>
/// Represents a time slot suitable for a meeting across multiple timezones.
/// </summary>
public class MeetingSlot
{
    /// <summary>
    /// Gets or sets the start time of the meeting slot (in UTC).
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the meeting slot (in UTC).
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the duration of the meeting slot.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeetingSlot"/> class.
    /// </summary>
    public MeetingSlot()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeetingSlot"/> class.
    /// </summary>
    /// <param name="startTime">The start time (in UTC).</param>
    /// <param name="endTime">The end time (in UTC).</param>
    public MeetingSlot(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Gets the start time in a specific timezone.
    /// </summary>
    /// <param name="timeZoneId">The timezone ID.</param>
    /// <returns>The start time in the specified timezone.</returns>
    public DateTime GetStartTimeInZone(string timeZoneId)
    {
        return TimeZoneKit.Convert(StartTime, timeZoneId);
    }

    /// <summary>
    /// Gets the end time in a specific timezone.
    /// </summary>
    /// <param name="timeZoneId">The timezone ID.</param>
    /// <returns>The end time in the specified timezone.</returns>
    public DateTime GetEndTimeInZone(string timeZoneId)
    {
        return TimeZoneKit.Convert(EndTime, timeZoneId);
    }

    /// <summary>
    /// Returns a string representation of this meeting slot.
    /// </summary>
    public override string ToString()
    {
        return $"{StartTime:yyyy-MM-dd HH:mm} UTC - {EndTime:HH:mm} UTC ({Duration.TotalHours:F1}h)";
    }
}
