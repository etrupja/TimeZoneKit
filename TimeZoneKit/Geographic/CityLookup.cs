using TimeZoneKit.Core;

namespace TimeZoneKit.Geographic;

/// <summary>
/// Provides city-based timezone lookups.
/// </summary>
internal static class CityLookup
{
    /// <summary>
    /// Gets the timezone ID for a given city name.
    /// </summary>
    /// <param name="cityName">The city name.</param>
    /// <returns>The IANA timezone ID.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when the city is not found.</exception>
    public static string GetTimeZoneByCity(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
        {
            throw new ArgumentException("City name cannot be null or empty.", nameof(cityName));
        }

        // Try exact match (case-insensitive)
        var match = TimeZoneMappings.CityMappings
            .FirstOrDefault(kvp => kvp.Key.Equals(cityName, StringComparison.OrdinalIgnoreCase));

        if (!match.Equals(default(KeyValuePair<string, string>)))
        {
            return match.Value;
        }

        // Try partial match
        var partialMatch = TimeZoneMappings.CityMappings
            .FirstOrDefault(kvp => kvp.Key.IndexOf(cityName, StringComparison.OrdinalIgnoreCase) >= 0);

        if (!partialMatch.Equals(default(KeyValuePair<string, string>)))
        {
            return partialMatch.Value;
        }

        throw new TimeZoneNotFoundException($"City not found: {cityName}");
    }

    /// <summary>
    /// Tries to get the timezone ID for a given city name.
    /// </summary>
    /// <param name="cityName">The city name.</param>
    /// <param name="timeZoneId">The IANA timezone ID, or null if not found.</param>
    /// <returns>True if the city was found; otherwise, false.</returns>
    public static bool TryGetTimeZoneByCity(string cityName, out string? timeZoneId)
    {
        try
        {
            timeZoneId = GetTimeZoneByCity(cityName);
            return true;
        }
        catch
        {
            timeZoneId = null;
            return false;
        }
    }
}
