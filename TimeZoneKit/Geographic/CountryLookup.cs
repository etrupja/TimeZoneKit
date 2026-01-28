using TimeZoneKit.Core;

namespace TimeZoneKit.Geographic;

/// <summary>
/// Provides country-based timezone lookups.
/// </summary>
internal static class CountryLookup
{
    /// <summary>
    /// Gets all timezone IDs for a given country code.
    /// </summary>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code (e.g., "US", "GB").</param>
    /// <returns>Array of IANA timezone IDs for the country.</returns>
    /// <exception cref="ArgumentException">Thrown when the country code is not found.</exception>
    public static string[] GetTimeZonesByCountry(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));
        }

        countryCode = countryCode.ToUpperInvariant();

        if (TimeZoneMappings.CountryMappings.TryGetValue(countryCode, out var timezones))
        {
            return timezones;
        }

        throw new ArgumentException($"Country code not found: {countryCode}", nameof(countryCode));
    }

    /// <summary>
    /// Tries to get timezone IDs for a given country code.
    /// </summary>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code.</param>
    /// <param name="timezones">Array of timezone IDs, or null if not found.</param>
    /// <returns>True if the country was found; otherwise, false.</returns>
    public static bool TryGetTimeZonesByCountry(string countryCode, out string[]? timezones)
    {
        try
        {
            timezones = GetTimeZonesByCountry(countryCode);
            return true;
        }
        catch
        {
            timezones = null;
            return false;
        }
    }
}
