namespace TimeZoneKit.Models;

/// <summary>
/// Represents timezone data including mappings, display names, and DST information.
/// </summary>
public class TimeZoneData
{
    /// <summary>
    /// Gets or sets the Windows timezone ID.
    /// </summary>
    public string Windows { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timezone abbreviations (e.g., EST, EDT).
    /// </summary>
    public string[] Abbreviations { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the friendly display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base UTC offset (e.g., "-05:00").
    /// </summary>
    public string BaseOffset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this timezone supports daylight saving time.
    /// </summary>
    public bool SupportsDst { get; set; }

    /// <summary>
    /// Gets or sets the country codes where this timezone is used.
    /// </summary>
    public string[] Countries { get; set; } = Array.Empty<string>();
}
