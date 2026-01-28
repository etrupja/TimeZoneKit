namespace TimeZoneKit.Tests;

public class MappingTests
{
    [Theory]
    [InlineData("America/New_York", "Eastern Standard Time")]
    [InlineData("America/Chicago", "Central Standard Time")]
    [InlineData("America/Los_Angeles", "Pacific Standard Time")]
    [InlineData("Europe/London", "GMT Standard Time")]
    [InlineData("Asia/Tokyo", "Tokyo Standard Time")]
    public void IanaToWindows_ReturnsCorrectMapping(string ianaId, string expectedWindowsId)
    {
        var windowsId = TimeZoneKit.IanaToWindows(ianaId);
        Assert.Equal(expectedWindowsId, windowsId);
    }

    [Theory]
    [InlineData("Eastern Standard Time", "America/New_York")]
    [InlineData("Central Standard Time", "America/Chicago")]
    [InlineData("Pacific Standard Time", "America/Los_Angeles")]
    [InlineData("GMT Standard Time", "Europe/London")]
    [InlineData("Tokyo Standard Time", "Asia/Tokyo")]
    public void WindowsToIana_ReturnsCorrectMapping(string windowsId, string expectedIanaId)
    {
        var ianaId = TimeZoneKit.WindowsToIana(windowsId);
        Assert.Equal(expectedIanaId, ianaId);
    }

    [Fact]
    public void IanaToWindows_InvalidId_ReturnsNull()
    {
        var result = TimeZoneKit.IanaToWindows("Invalid/Zone");
        Assert.Null(result);
    }

    [Fact]
    public void WindowsToIana_InvalidId_ReturnsNull()
    {
        var result = TimeZoneKit.WindowsToIana("Invalid Zone");
        Assert.Null(result);
    }

    [Fact]
    public void IanaToWindows_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TimeZoneKit.IanaToWindows(null!));
    }

    [Fact]
    public void WindowsToIana_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TimeZoneKit.WindowsToIana(null!));
    }

    [Theory]
    [InlineData("America/New_York")]
    [InlineData("America/Chicago")]
    [InlineData("Europe/London")]
    [InlineData("Asia/Tokyo")]
    public void GetTimeZoneInfo_IanaId_ReturnsTimeZoneInfo(string ianaId)
    {
        var tzInfo = TimeZoneKit.GetTimeZoneInfo(ianaId);
        Assert.NotNull(tzInfo);
    }

    [Theory]
    [InlineData("Eastern Standard Time")]
    [InlineData("Pacific Standard Time")]
    [InlineData("GMT Standard Time")]
    public void GetTimeZoneInfo_WindowsId_ReturnsTimeZoneInfo(string windowsId)
    {
        var tzInfo = TimeZoneKit.GetTimeZoneInfo(windowsId);
        Assert.NotNull(tzInfo);
    }

    [Fact]
    public void GetTimeZoneInfo_InvalidId_ThrowsTimeZoneNotFoundException()
    {
        Assert.Throws<TimeZoneNotFoundException>(() => TimeZoneKit.GetTimeZoneInfo("Invalid/Zone"));
    }

    [Theory]
    [InlineData("America/New_York", "Eastern Time (US & Canada)")]
    [InlineData("Europe/London", "Greenwich Mean Time")]
    [InlineData("Asia/Tokyo", "Japan Standard Time")]
    public void GetFriendlyName_ReturnsDisplayName(string timeZoneId, string expectedName)
    {
        var displayName = TimeZoneKit.GetFriendlyName(timeZoneId);
        Assert.Equal(expectedName, displayName);
    }
}
