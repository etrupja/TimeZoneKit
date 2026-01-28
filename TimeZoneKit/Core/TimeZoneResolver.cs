namespace TimeZoneKit.Core;

/// <summary>
/// Resolves timezone identifiers between IANA and Windows formats.
/// </summary>
internal static class TimeZoneResolver
{
    /// <summary>
    /// Converts an IANA timezone ID to a Windows timezone ID.
    /// </summary>
    /// <param name="ianaId">The IANA timezone identifier.</param>
    /// <returns>The Windows timezone ID, or null if not found.</returns>
    public static string? IanaToWindows(string ianaId)
    {
        if (TimeZoneMappings.Mappings.TryGetValue(ianaId, out var data))
        {
            return data.Windows;
        }

        return null;
    }

    /// <summary>
    /// Converts a Windows timezone ID to an IANA timezone ID.
    /// </summary>
    /// <param name="windowsId">The Windows timezone identifier.</param>
    /// <returns>The IANA timezone ID, or null if not found.</returns>
    public static string? WindowsToIana(string windowsId)
    {
        foreach (var kvp in TimeZoneMappings.Mappings)
        {
            if (kvp.Value.Windows.Equals(windowsId, StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Key;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a TimeZoneInfo for the given timezone identifier (supports both IANA and Windows IDs).
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier (IANA or Windows).</param>
    /// <returns>The TimeZoneInfo instance.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when the timezone ID is not found.</exception>
    public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            throw new ArgumentException("Timezone ID cannot be null or empty.", nameof(timeZoneId));
        }

        return TimeZoneCache.GetOrAdd(timeZoneId, id =>
        {
            // Try to find it as-is (works on non-Windows systems with IANA IDs)
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
                // Continue to fallback options
            }

            // Try converting IANA to Windows
            var windowsId = IanaToWindows(id);
            if (windowsId != null)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                }
                catch (TimeZoneNotFoundException)
                {
                    // Continue to next fallback
                }
            }

            // Try converting Windows to IANA
            var ianaId = WindowsToIana(id);
            if (ianaId != null)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
                }
                catch (TimeZoneNotFoundException)
                {
                    // Continue to final exception
                }
            }

            throw new TimeZoneNotFoundException($"Timezone not found: {id}");
        });
    }

    /// <summary>
    /// Tries to get a TimeZoneInfo for the given timezone identifier.
    /// </summary>
    /// <param name="timeZoneId">The timezone identifier.</param>
    /// <param name="timeZoneInfo">The resolved TimeZoneInfo, or null if not found.</param>
    /// <returns>True if the timezone was found; otherwise, false.</returns>
    public static bool TryGetTimeZoneInfo(string timeZoneId, out TimeZoneInfo? timeZoneInfo)
    {
        try
        {
            timeZoneInfo = GetTimeZoneInfo(timeZoneId);
            return true;
        }
        catch
        {
            timeZoneInfo = null;
            return false;
        }
    }
}
