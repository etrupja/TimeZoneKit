using System.Text.RegularExpressions;
using TimeZoneKit.Models;

namespace TimeZoneKit.Core;

/// <summary>
/// Parses timezone identifiers from various formats including abbreviations, city names, and offsets.
/// </summary>
internal static class TimeZoneParser
{
    private static readonly Regex OffsetRegex = new(@"^GMT([+-]\d{1,2})(?::?(\d{2}))?$|^UTC([+-]\d{1,2})(?::?(\d{2}))?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Parses a timezone identifier from various input formats.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <returns>The resolved IANA timezone ID.</returns>
    /// <exception cref="TimeZoneNotFoundException">Thrown when the timezone cannot be resolved.</exception>
    public static string Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Timezone input cannot be null or empty.", nameof(input));
        }

        input = input.Trim();

        // Try direct IANA ID match
        if (TimeZoneMappings.Mappings.ContainsKey(input))
        {
            return input;
        }

        // Try Windows ID to IANA conversion
        var ianaFromWindows = TimeZoneResolver.WindowsToIana(input);
        if (ianaFromWindows != null)
        {
            return ianaFromWindows;
        }

        // Try abbreviation lookup (case-insensitive)
        var abbreviationMatch = TimeZoneMappings.Abbreviations
            .FirstOrDefault(kvp => kvp.Key.Equals(input, StringComparison.OrdinalIgnoreCase));
        if (!abbreviationMatch.Equals(default(KeyValuePair<string, string>)))
        {
            return abbreviationMatch.Value;
        }

        // Try city name lookup (case-insensitive)
        var cityMatch = TimeZoneMappings.CityMappings
            .FirstOrDefault(kvp => kvp.Key.Equals(input, StringComparison.OrdinalIgnoreCase));
        if (!cityMatch.Equals(default(KeyValuePair<string, string>)))
        {
            return cityMatch.Value;
        }

        // Try fuzzy display name matching
        var displayNameMatch = TimeZoneMappings.Mappings
            .FirstOrDefault(kvp => kvp.Value.DisplayName.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0);
        if (!displayNameMatch.Equals(default(KeyValuePair<string, TimeZoneData>)))
        {
            return displayNameMatch.Key;
        }

        // Try offset parsing (GMT-5, UTC+3, etc.)
        var offsetMatch = OffsetRegex.Match(input);
        if (offsetMatch.Success)
        {
            var offsetHours = int.Parse(offsetMatch.Groups[1].Success ? offsetMatch.Groups[1].Value : offsetMatch.Groups[3].Value);
            var offsetMinutes = 0;
            if (offsetMatch.Groups[2].Success && !string.IsNullOrEmpty(offsetMatch.Groups[2].Value))
            {
                offsetMinutes = int.Parse(offsetMatch.Groups[2].Value);
            }
            else if (offsetMatch.Groups[4].Success && !string.IsNullOrEmpty(offsetMatch.Groups[4].Value))
            {
                offsetMinutes = int.Parse(offsetMatch.Groups[4].Value);
            }

            var offset = new TimeSpan(offsetHours, offsetMinutes, 0);
            var matchingZone = FindByOffset(offset);
            if (matchingZone != null)
            {
                return matchingZone;
            }
        }

        throw new TimeZoneNotFoundException($"Could not parse timezone: {input}");
    }

    /// <summary>
    /// Tries to parse a timezone identifier.
    /// </summary>
    /// <param name="input">The input string to parse.</param>
    /// <param name="timeZoneId">The resolved IANA timezone ID, or null if parsing failed.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParse(string input, out string? timeZoneId)
    {
        try
        {
            timeZoneId = Parse(input);
            return true;
        }
        catch
        {
            timeZoneId = null;
            return false;
        }
    }

    /// <summary>
    /// Searches for timezones matching the given query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>List of matching IANA timezone IDs.</returns>
    public static List<string> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<string>();
        }

        query = query.Trim();
        var results = new HashSet<string>();

        // Search IANA IDs
        foreach (var ianaId in TimeZoneMappings.Mappings.Keys)
        {
            if (ianaId.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                results.Add(ianaId);
            }
        }

        // Search display names
        foreach (var kvp in TimeZoneMappings.Mappings)
        {
            if (kvp.Value.DisplayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                results.Add(kvp.Key);
            }
        }

        // Search abbreviations
        foreach (var kvp in TimeZoneMappings.Abbreviations)
        {
            if (kvp.Key.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                results.Add(kvp.Value);
            }
        }

        // Search cities
        foreach (var kvp in TimeZoneMappings.CityMappings)
        {
            if (kvp.Key.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                results.Add(kvp.Value);
            }
        }

        return results.ToList();
    }

    private static string? FindByOffset(TimeSpan offset)
    {
        // Find a timezone that matches the given offset
        foreach (var kvp in TimeZoneMappings.Mappings)
        {
            // Remove leading '+' sign if present, as TimeSpan.TryParse doesn't handle it
            var offsetStr = kvp.Value.BaseOffset?.TrimStart('+') ?? string.Empty;

            if (TimeSpan.TryParse(offsetStr, out var baseOffset))
            {
                if (baseOffset == offset)
                {
                    return kvp.Key;
                }
            }
        }

        return null;
    }
}
