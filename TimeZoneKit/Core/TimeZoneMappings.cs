using System.Reflection;
using System.Text.Json;
using TimeZoneKit.Models;

namespace TimeZoneKit.Core;

/// <summary>
/// Manages loading and caching of timezone mapping data from embedded resources.
/// </summary>
internal static class TimeZoneMappings
{
    private static readonly Lazy<Dictionary<string, TimeZoneData>> _mappings = new(LoadMappings);
    private static readonly Lazy<Dictionary<string, string>> _abbreviations = new(LoadAbbreviations);
    private static readonly Lazy<Dictionary<string, string>> _cityMappings = new(LoadCityMappings);
    private static readonly Lazy<Dictionary<string, string[]>> _countryMappings = new(LoadCountryMappings);
    private static readonly Lazy<string[]> _commonTimezones = new(LoadCommonTimezones);
    private static readonly Lazy<Dictionary<string, string>> _displayNames = new(LoadDisplayNames);

    /// <summary>
    /// Gets all timezone mappings (IANA ID -> TimeZoneData).
    /// </summary>
    public static Dictionary<string, TimeZoneData> Mappings => _mappings.Value;

    /// <summary>
    /// Gets abbreviation to IANA ID mappings.
    /// </summary>
    public static Dictionary<string, string> Abbreviations => _abbreviations.Value;

    /// <summary>
    /// Gets city name to IANA ID mappings.
    /// </summary>
    public static Dictionary<string, string> CityMappings => _cityMappings.Value;

    /// <summary>
    /// Gets country code to IANA IDs mappings.
    /// </summary>
    public static Dictionary<string, string[]> CountryMappings => _countryMappings.Value;

    /// <summary>
    /// Gets the list of common timezones.
    /// </summary>
    public static string[] CommonTimezones => _commonTimezones.Value;

    /// <summary>
    /// Gets display names for timezones.
    /// </summary>
    public static Dictionary<string, string> DisplayNames => _displayNames.Value;

    private static Dictionary<string, TimeZoneData> LoadMappings()
    {
        var json = LoadEmbeddedResource("timezone-mappings.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<Dictionary<string, TimeZoneData>>(json, options)
               ?? new Dictionary<string, TimeZoneData>();
    }

    private static Dictionary<string, string> LoadAbbreviations()
    {
        var json = LoadEmbeddedResource("abbreviations.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json, options)
               ?? new Dictionary<string, string>();
    }

    private static Dictionary<string, string> LoadCityMappings()
    {
        var json = LoadEmbeddedResource("city-timezones.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json, options)
               ?? new Dictionary<string, string>();
    }

    private static Dictionary<string, string[]> LoadCountryMappings()
    {
        var json = LoadEmbeddedResource("country-timezones.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<Dictionary<string, string[]>>(json, options)
               ?? new Dictionary<string, string[]>();
    }

    private static string[] LoadCommonTimezones()
    {
        var json = LoadEmbeddedResource("common-timezones.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<string[]>(json, options)
               ?? Array.Empty<string>();
    }

    private static Dictionary<string, string> LoadDisplayNames()
    {
        var json = LoadEmbeddedResource("display-names-en.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json, options)
               ?? new Dictionary<string, string>();
    }

    private static string LoadEmbeddedResource(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"TimeZoneKit.Data.{fileName}";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
