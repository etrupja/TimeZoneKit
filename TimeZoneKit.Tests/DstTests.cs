using TimeZoneKit.Extensions;

namespace TimeZoneKit.Tests;

public class DstTests
{
    [Theory]
    [InlineData("America/New_York", true)]
    [InlineData("America/Chicago", true)]
    [InlineData("Europe/London", true)]
    [InlineData("Asia/Tokyo", false)]
    [InlineData("Pacific/Honolulu", false)]
    [InlineData("Asia/Shanghai", false)]
    [InlineData("Asia/Dubai", false)]
    public void SupportsDst_ReturnsCorrectValue(string timeZoneId, bool expectedSupportsDst)
    {
        var supportsDst = TimeZoneKit.SupportsDst(timeZoneId);
        Assert.Equal(expectedSupportsDst, supportsDst);
    }

    [Fact]
    public void IsDaylightSavingTime_WinterTime_ReturnsFalse()
    {
        var winterTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var isDst = TimeZoneKit.IsDaylightSavingTime("America/New_York", winterTime);
        Assert.False(isDst);
    }

    [Fact]
    public void IsDaylightSavingTime_SummerTime_ReturnsTrue()
    {
        var summerTime = new DateTime(2025, 7, 15, 12, 0, 0, DateTimeKind.Utc);
        var isDst = TimeZoneKit.IsDaylightSavingTime("America/New_York", summerTime);
        Assert.True(isDst);
    }

    [Fact]
    public void GetOffsetAt_WinterTime_ReturnsCorrectOffset()
    {
        var winterTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var offset = TimeZoneKit.GetOffsetAt("America/New_York", winterTime);
        Assert.Equal(-5, offset.TotalHours); // EST is UTC-5
    }

    [Fact]
    public void GetOffsetAt_SummerTime_ReturnsCorrectOffset()
    {
        var summerTime = new DateTime(2025, 7, 15, 12, 0, 0, DateTimeKind.Utc);
        var offset = TimeZoneKit.GetOffsetAt("America/New_York", summerTime);
        Assert.Equal(-4, offset.TotalHours); // EDT is UTC-4
    }

    [Fact]
    public void GetOffset_ExtensionMethod_WorksCorrectly()
    {
        var dateTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var offset = dateTime.GetOffset("America/New_York");
        Assert.Equal(-5, offset.TotalHours);
    }

    [Fact]
    public void IsDaylightSavingTime_ExtensionMethod_WorksCorrectly()
    {
        var winterTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var isDst = winterTime.IsDaylightSavingTime("America/New_York");
        Assert.False(isDst);
    }

    [Fact]
    public void SupportsDst_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TimeZoneKit.SupportsDst(null!));
    }

    [Theory]
    [InlineData("Asia/Tokyo", 9)]
    [InlineData("Europe/London", 0)]
    [InlineData("Pacific/Honolulu", -10)]
    public void GetOffsetAt_NoDestZones_ReturnsStaticOffset(string timeZoneId, int expectedHours)
    {
        var dateTime = DateTime.UtcNow;
        var offset = TimeZoneKit.GetOffsetAt(timeZoneId, dateTime);
        Assert.Equal(expectedHours, offset.TotalHours);
    }

    [Theory]
    [InlineData("America/Chicago")] // CST/CDT
    [InlineData("America/Denver")] // MST/MDT
    [InlineData("America/Los_Angeles")] // PST/PDT
    [InlineData("Europe/Paris")] // CET/CEST
    [InlineData("Europe/Berlin")] // CET/CEST
    [InlineData("Australia/Sydney")] // AEST/AEDT
    [InlineData("Pacific/Auckland")] // NZST/NZDT
    public void SupportsDst_DstTimezones_ReturnsTrue(string timeZoneId)
    {
        var supportsDst = TimeZoneKit.SupportsDst(timeZoneId);
        Assert.True(supportsDst);
    }

    [Theory]
    [InlineData("Asia/Tokyo")]
    [InlineData("Asia/Shanghai")]
    [InlineData("Asia/Dubai")]
    [InlineData("Asia/Kolkata")]
    [InlineData("Asia/Singapore")]
    [InlineData("Pacific/Honolulu")]
    [InlineData("America/Phoenix")]
    [InlineData("Africa/Johannesburg")]
    [InlineData("Asia/Bangkok")]
    [InlineData("Asia/Seoul")]
    public void SupportsDst_NoDstTimezones_ReturnsFalse(string timeZoneId)
    {
        var supportsDst = TimeZoneKit.SupportsDst(timeZoneId);
        Assert.False(supportsDst);
    }

    [Theory]
    [InlineData("America/New_York", "2025-01-15")]
    [InlineData("America/Chicago", "2025-02-15")]
    [InlineData("America/Denver", "2025-12-15")]
    [InlineData("Europe/London", "2025-01-15")]
    [InlineData("Europe/Paris", "2025-02-15")]
    [InlineData("Australia/Sydney", "2025-06-15")] // Winter in Australia
    public void IsDaylightSavingTime_WinterMonths_ReturnsFalse(string timeZoneId, string dateStr)
    {
        var date = DateTime.Parse(dateStr).ToUniversalTime();
        var isDst = TimeZoneKit.IsDaylightSavingTime(timeZoneId, date);
        Assert.False(isDst);
    }

    [Theory]
    [InlineData("America/New_York", "2025-06-15")]
    [InlineData("America/Chicago", "2025-07-15")]
    [InlineData("America/Denver", "2025-08-15")]
    [InlineData("Europe/London", "2025-06-15")]
    [InlineData("Europe/Paris", "2025-07-15")]
    [InlineData("Australia/Sydney", "2025-12-15")] // Summer in Australia
    public void IsDaylightSavingTime_SummerMonths_ReturnsTrue(string timeZoneId, string dateStr)
    {
        var date = DateTime.Parse(dateStr).ToUniversalTime();
        var isDst = TimeZoneKit.IsDaylightSavingTime(timeZoneId, date);
        Assert.True(isDst);
    }

    [Fact]
    public void GetOffsetAt_DifferentTimesInYear_ReflectsDst()
    {
        var winter = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var summer = new DateTime(2025, 7, 15, 12, 0, 0, DateTimeKind.Utc);

        var winterOffset = TimeZoneKit.GetOffsetAt("America/New_York", winter);
        var summerOffset = TimeZoneKit.GetOffsetAt("America/New_York", summer);

        // Winter should be -5 (EST), Summer should be -4 (EDT)
        Assert.Equal(-5, winterOffset.TotalHours);
        Assert.Equal(-4, summerOffset.TotalHours);
    }

    [Fact]
    public void GetOffsetAt_AustralianTimezone_ReverseDst()
    {
        // In Australia, DST is during their summer (December-January)
        var australianWinter = new DateTime(2025, 6, 15, 12, 0, 0, DateTimeKind.Utc);
        var australianSummer = new DateTime(2025, 12, 15, 12, 0, 0, DateTimeKind.Utc);

        var winterOffset = TimeZoneKit.GetOffsetAt("Australia/Sydney", australianWinter);
        var summerOffset = TimeZoneKit.GetOffsetAt("Australia/Sydney", australianSummer);

        // Summer offset should be greater (AEDT vs AEST)
        Assert.True(summerOffset.TotalHours > winterOffset.TotalHours);
    }

    [Theory]
    [InlineData("America/St_Johns", -3.5)] // Newfoundland Standard Time
    [InlineData("Asia/Kolkata", 5.5)] // India Standard Time
    [InlineData("Asia/Kathmandu", 5.75)] // Nepal Time
    [InlineData("Australia/Adelaide", 10.5)] // Australian Central Standard Time (summer - DST active in Jan)
    public void GetOffsetAt_NonStandardOffsets_ReturnsCorrectly(string timeZoneId, double expectedHours)
    {
        var winterTime = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        var offset = TimeZoneKit.GetOffsetAt(timeZoneId, winterTime);

        Assert.Equal(expectedHours, offset.TotalHours);
    }
}
