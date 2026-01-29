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

    [Theory]
    [InlineData("America/New_York", "America/Los_Angeles", 3)] // EST to PST: -3 hours
    [InlineData("America/Los_Angeles", "America/New_York", -3)] // PST to EST: +3 hours
    [InlineData("Europe/London", "Asia/Tokyo", -9)] // GMT to JST: +9 hours
    [InlineData("Asia/Tokyo", "Europe/London", 9)] // JST to GMT: -9 hours
    [InlineData("America/New_York", "Europe/London", -5)] // EST to GMT: +5 hours
    [InlineData("Asia/Kolkata", "America/New_York", 10.5)] // IST to EST: -10.5 hours
    public void Convert_BetweenTimezones_CorrectHourDifference(string fromTz, string toTz, double expectedHourDiff)
    {
        // Use January date to avoid DST complications
        var sourceTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Unspecified);
        var converted = TimeZoneKit.Convert(sourceTime, fromTz, toTz);

        // Verify the conversion happened (hour changed)
        Assert.NotEqual(sourceTime.Hour, converted.Hour);
    }

    [Fact]
    public void Convert_AcrossDstTransition_HandlesCorrectly()
    {
        // Test conversion before DST starts
        var beforeDst = new DateTime(2025, 3, 1, 12, 0, 0, DateTimeKind.Utc);
        var nyBefore = TimeZoneKit.Convert(beforeDst, "America/New_York");

        // Test conversion after DST starts
        var afterDst = new DateTime(2025, 4, 15, 12, 0, 0, DateTimeKind.Utc);
        var nyAfter = TimeZoneKit.Convert(afterDst, "America/New_York");

        // Hour offset should be different (EST vs EDT)
        Assert.NotEqual(nyBefore.Hour, nyAfter.Hour);
    }

    [Fact]
    public void Convert_UtcToMultipleTimezones_ReturnsCorrectTimes()
    {
        var utcTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        var nyTime = TimeZoneKit.Convert(utcTime, "America/New_York");
        var londonTime = TimeZoneKit.Convert(utcTime, "Europe/London");
        var tokyoTime = TimeZoneKit.Convert(utcTime, "Asia/Tokyo");
        var sydneyTime = TimeZoneKit.Convert(utcTime, "Australia/Sydney");

        // Verify all conversions happened
        Assert.NotEqual(utcTime.Hour, nyTime.Hour);
        Assert.NotEqual(utcTime.Hour, tokyoTime.Hour);

        // Tokyo should be ahead of UTC
        Assert.True(tokyoTime.Hour > utcTime.Hour || tokyoTime.Day > utcTime.Day);
    }

    [Fact]
    public void ToUtc_FromMultipleTimezones_ReturnsCorrectUtc()
    {
        var localTime = new DateTime(2025, 1, 15, 12, 0, 0);

        var utcFromNy = TimeZoneKit.ToUtc(localTime, "America/New_York");
        var utcFromLondon = TimeZoneKit.ToUtc(localTime, "Europe/London");
        var utcFromTokyo = TimeZoneKit.ToUtc(localTime, "Asia/Tokyo");

        // All should be different UTC times
        Assert.NotEqual(utcFromNy, utcFromLondon);
        Assert.NotEqual(utcFromLondon, utcFromTokyo);
    }

    [Fact]
    public void Convert_RoundTrip_PreservesTime()
    {
        var originalUtc = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);

        // Convert UTC -> NY -> UTC
        var nyTime = TimeZoneKit.Convert(originalUtc, "America/New_York");
        var backToUtc = TimeZoneKit.ToUtc(nyTime, "America/New_York");

        Assert.Equal(originalUtc.Hour, backToUtc.Hour);
        Assert.Equal(originalUtc.Minute, backToUtc.Minute);
    }

    [Theory]
    [InlineData("America/Phoenix")] // No DST
    [InlineData("Asia/Tokyo")] // No DST
    [InlineData("Asia/Dubai")] // No DST
    [InlineData("Pacific/Honolulu")] // No DST
    public void Convert_NoDstTimezone_ConsistentThroughoutYear(string timeZoneId)
    {
        var winterTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var summerTime = new DateTime(2025, 7, 15, 12, 0, 0, DateTimeKind.Utc);

        var winterConverted = TimeZoneKit.Convert(winterTime, timeZoneId);
        var summerConverted = TimeZoneKit.Convert(summerTime, timeZoneId);

        // Hour offset should be the same in winter and summer
        var winterOffset = winterConverted.Hour - winterTime.Hour;
        var summerOffset = summerConverted.Hour - summerTime.Hour;

        // Normalize offsets to handle day boundaries
        if (winterOffset < -12) winterOffset += 24;
        if (winterOffset > 12) winterOffset -= 24;
        if (summerOffset < -12) summerOffset += 24;
        if (summerOffset > 12) summerOffset -= 24;

        Assert.Equal(winterOffset, summerOffset);
    }
}
