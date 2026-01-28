namespace TimeZoneKit.Extensions;

/// <summary>
/// Extension methods for DateTime to simplify timezone operations.
/// </summary>
public static class DateTimeExtensions
{
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
    /// var nyTime = utcTime.ToTimeZone("America/New_York");
    /// </code>
    /// </example>
    public static DateTime ToTimeZone(this DateTime dateTime, string timeZoneId)
    {
        return TimeZoneKit.Convert(dateTime, timeZoneId);
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
    /// var tokyoTime = localTime.ToTimeZone("America/New_York", "Asia/Tokyo");
    /// </code>
    /// </example>
    public static DateTime ToTimeZone(this DateTime dateTime, string fromTimeZoneId, string toTimeZoneId)
    {
        return TimeZoneKit.Convert(dateTime, fromTimeZoneId, toTimeZoneId);
    }

    /// <summary>
    /// Converts the DateTime to UTC from the specified timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <param name="timeZoneId">The timezone ID of the DateTime (IANA or Windows).</param>
    /// <returns>The DateTime in UTC.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
    /// var utcTime = localTime.ToUtc("America/New_York");
    /// </code>
    /// </example>
    public static DateTime ToUtc(this DateTime dateTime, string timeZoneId)
    {
        return TimeZoneKit.ToUtc(dateTime, timeZoneId);
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
    /// var offset = DateTime.Now.GetOffset("America/New_York");
    /// Console.WriteLine($"Current offset: {offset.TotalHours} hours");
    /// </code>
    /// </example>
    public static TimeSpan GetOffset(this DateTime dateTime, string timeZoneId)
    {
        return TimeZoneKit.GetOffsetAt(timeZoneId, dateTime);
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
    /// var isDst = DateTime.Now.IsDaylightSavingTime("America/New_York");
    /// </code>
    /// </example>
    public static bool IsDaylightSavingTime(this DateTime dateTime, string timeZoneId)
    {
        return TimeZoneKit.IsDaylightSavingTime(timeZoneId, dateTime);
    }

    /// <summary>
    /// Checks if the DateTime is during standard business hours in the specified timezone.
    /// </summary>
    /// <param name="dateTime">The DateTime to check.</param>
    /// <param name="timeZoneId">The timezone ID (IANA or Windows).</param>
    /// <param name="startHour">Business day start hour (default: 9).</param>
    /// <param name="endHour">Business day end hour (default: 17).</param>
    /// <returns>True if during business hours (weekdays, 9-5); otherwise, false.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var isOpen = DateTime.UtcNow.IsBusinessHour("America/New_York");
    /// </code>
    /// </example>
    public static bool IsBusinessHour(this DateTime dateTime, string timeZoneId, int startHour = 9, int endHour = 17)
    {
        return TimeZoneKit.IsBusinessHour(dateTime, timeZoneId, startHour, endHour);
    }

    /// <summary>
    /// Finds the next business hour from the DateTime in the specified timezone.
    /// </summary>
    /// <param name="dateTime">The starting DateTime.</param>
    /// <param name="timeZoneId">The timezone ID (IANA or Windows).</param>
    /// <param name="startHour">Business day start hour (default: 9).</param>
    /// <param name="endHour">Business day end hour (default: 17).</param>
    /// <returns>The next business hour DateTime, or null if none found within 7 days.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when timezone ID is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown when timeZoneId is null.</exception>
    /// <example>
    /// <code>
    /// var nextOpen = DateTime.UtcNow.NextBusinessHour("America/New_York");
    /// </code>
    /// </example>
    public static DateTime? NextBusinessHour(this DateTime dateTime, string timeZoneId, int startHour = 9, int endHour = 17)
    {
        return TimeZoneKit.NextBusinessHour(dateTime, timeZoneId, startHour, endHour);
    }
}
