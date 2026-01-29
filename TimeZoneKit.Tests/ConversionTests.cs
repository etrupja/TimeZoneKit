using TimeZoneKit.Extensions;

namespace TimeZoneKit.Tests;

public class ConversionTests
{
    [Fact]
    public void Convert_UtcToEastern_ReturnsCorrectTime()
    {
        // January - EST (no DST)
        var utcTime = new DateTime(2025, 1, 28, 15, 0, 0, DateTimeKind.Utc);
        var eastTime = TimeZoneKit.Convert(utcTime, "America/New_York");

        Assert.Equal(10, eastTime.Hour); // UTC-5 during standard time
    }

    [Fact]
    public void Convert_UtcToEasternDuringDST_ReturnsCorrectTime()
    {
        // July - EDT (DST active)
        var utcTime = new DateTime(2025, 7, 15, 15, 0, 0, DateTimeKind.Utc);
        var eastTime = TimeZoneKit.Convert(utcTime, "America/New_York");

        Assert.Equal(11, eastTime.Hour); // UTC-4 during daylight time
    }
    
    [Fact]
    public void Convert_BetweenTimezones_ReturnsCorrectTime()
    {
        var nyTime = new DateTime(2025, 1, 28, 10, 0, 0, DateTimeKind.Unspecified);
        var tokyoTime = TimeZoneKit.Convert(nyTime, "America/New_York", "Asia/Tokyo");

        // NY is UTC-5, Tokyo is UTC+9, difference is 14 hours
        Assert.Equal(29, tokyoTime.Day); // Next day (Jan 29)
        Assert.Equal(0, tokyoTime.Hour); // Midnight
        Assert.Equal(new DateTime(2025, 1, 29, 0, 0, 0), tokyoTime);
    }

    [Fact]
    public void ToUtc_FromEastern_ReturnsCorrectTime()
    {
        var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
        var utcTime = TimeZoneKit.ToUtc(localTime, "America/New_York");

        Assert.Equal(15, utcTime.Hour); // 10 AM EST = 3 PM UTC
    }

    [Fact]
    public void Convert_NullTimeZoneId_ThrowsArgumentNullException()
    {
        var dateTime = DateTime.UtcNow;
        Assert.Throws<ArgumentNullException>(() => TimeZoneKit.Convert(dateTime, null!));
    }

    [Fact]
    public void Convert_InvalidTimeZoneId_ThrowsTimeZoneNotFoundException()
    {
        var dateTime = DateTime.UtcNow;
        Assert.Throws<TimeZoneNotFoundException>(() => TimeZoneKit.Convert(dateTime, "Invalid/Zone"));
    }

    [Fact]
    public void ToTimeZone_ExtensionMethod_WorksCorrectly()
    {
        var utcTime = new DateTime(2025, 1, 28, 15, 0, 0, DateTimeKind.Utc);
        var nyTime = utcTime.ToTimeZone("America/New_York");

        Assert.Equal(10, nyTime.Hour);
    }

    [Fact]
    public void ToUtc_ExtensionMethod_WorksCorrectly()
    {
        var localTime = new DateTime(2025, 1, 28, 10, 0, 0);
        var utcTime = localTime.ToUtc("America/New_York");

        Assert.Equal(15, utcTime.Hour);
    }

    [Theory]
    [InlineData("America/New_York", "Eastern Standard Time")]
    [InlineData("America/Chicago", "Central Standard Time")]
    [InlineData("America/Los_Angeles", "Pacific Standard Time")]
    [InlineData("Europe/London", "GMT Standard Time")]
    [InlineData("Asia/Tokyo", "Tokyo Standard Time")]
    public void Convert_WindowsId_WorksCorrectly(string ianaId, string windowsId)
    {
        var utcTime = DateTime.UtcNow;

        var result1 = TimeZoneKit.Convert(utcTime, ianaId);
        var result2 = TimeZoneKit.Convert(utcTime, windowsId);

        // Both should produce the same result
        Assert.Equal(result1.Hour, result2.Hour);
    }
}
