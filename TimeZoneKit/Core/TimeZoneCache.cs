using System.Collections.Concurrent;

namespace TimeZoneKit.Core;

/// <summary>
/// Thread-safe cache for TimeZoneInfo instances.
/// </summary>
internal static class TimeZoneCache
{
    private static readonly ConcurrentDictionary<string, TimeZoneInfo> _cache = new();

    /// <summary>
    /// Gets or adds a TimeZoneInfo instance to the cache.
    /// </summary>
    /// <param name="id">The timezone identifier.</param>
    /// <param name="factory">Factory function to create the TimeZoneInfo if not cached.</param>
    /// <returns>The cached or newly created TimeZoneInfo instance.</returns>
    public static TimeZoneInfo GetOrAdd(string id, Func<string, TimeZoneInfo> factory)
    {
        return _cache.GetOrAdd(id, factory);
    }

    /// <summary>
    /// Clears the cache (primarily for testing purposes).
    /// </summary>
    public static void Clear()
    {
        _cache.Clear();
    }
}
