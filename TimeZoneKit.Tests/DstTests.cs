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
}
