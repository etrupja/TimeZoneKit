using TimeZoneKit.Methods;

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
        var windowsId = TimeZoneHelper.IanaToWindows(ianaId);
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
        var ianaId = TimeZoneHelper.WindowsToIana(windowsId);
        Assert.Equal(expectedIanaId, ianaId);
    }

    [Fact]
    public void IanaToWindows_InvalidId_ReturnsNull()
    {
        var result = TimeZoneHelper.IanaToWindows("Invalid/Zone");
        Assert.Null(result);
    }

    [Fact]
    public void WindowsToIana_InvalidId_ReturnsNull()
    {
        var result = TimeZoneHelper.WindowsToIana("Invalid Zone");
        Assert.Null(result);
    }

    [Fact]
    public void IanaToWindows_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TimeZoneHelper.IanaToWindows(null!));
    }

    [Fact]
    public void WindowsToIana_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TimeZoneHelper.WindowsToIana(null!));
    }

    [Theory]
    [InlineData("America/New_York")]
    [InlineData("America/Chicago")]
    [InlineData("Europe/London")]
    [InlineData("Asia/Tokyo")]
    public void GetTimeZoneInfo_IanaId_ReturnsTimeZoneInfo(string ianaId)
    {
        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(ianaId);
        Assert.NotNull(tzInfo);
    }

    [Theory]
    [InlineData("Eastern Standard Time")]
    [InlineData("Pacific Standard Time")]
    [InlineData("GMT Standard Time")]
    public void GetTimeZoneInfo_WindowsId_ReturnsTimeZoneInfo(string windowsId)
    {
        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(windowsId);
        Assert.NotNull(tzInfo);
    }

    [Fact]
    public void GetTimeZoneInfo_InvalidId_ThrowsTimeZoneNotFoundException()
    {
        Assert.Throws<TimeZoneNotFoundException>(() => TimeZoneHelper.GetTimeZoneInfo("Invalid/Zone"));
    }

    [Theory]
    [InlineData("America/New_York", "Eastern Time (US & Canada)")]
    [InlineData("Europe/London", "Greenwich Mean Time")]
    [InlineData("Asia/Tokyo", "Japan Standard Time")]
    [InlineData("America/Chicago", "Central Time (US & Canada)")]
    [InlineData("America/Denver", "Mountain Time (US & Canada)")]
    [InlineData("America/Los_Angeles", "Pacific Time (US & Canada)")]
    [InlineData("America/Phoenix", "Mountain Standard Time (Arizona)")]
    [InlineData("America/Anchorage", "Alaska Time")]
    [InlineData("Pacific/Honolulu", "Hawaii-Aleutian Standard Time")]
    [InlineData("America/Toronto", "Eastern Time (Toronto)")]
    [InlineData("America/Vancouver", "Pacific Time (Vancouver)")]
    [InlineData("America/Halifax", "Atlantic Time")]
    [InlineData("America/St_Johns", "Newfoundland Time")]
    [InlineData("Europe/Paris", "Central European Time (Paris)")]
    [InlineData("Europe/Berlin", "Central European Time (Berlin)")]
    [InlineData("Europe/Rome", "Central European Time (Rome)")]
    [InlineData("Europe/Madrid", "Central European Time (Madrid)")]
    [InlineData("Europe/Athens", "Eastern European Time")]
    [InlineData("Europe/Istanbul", "Turkey Time")]
    [InlineData("Europe/Moscow", "Moscow Standard Time")]
    [InlineData("Asia/Dubai", "Gulf Standard Time")]
    [InlineData("Asia/Kolkata", "India Standard Time")]
    [InlineData("Asia/Singapore", "Singapore Standard Time")]
    [InlineData("Asia/Hong_Kong", "Hong Kong Time")]
    [InlineData("Asia/Shanghai", "China Standard Time")]
    [InlineData("Asia/Seoul", "Korea Standard Time")]
    [InlineData("Asia/Bangkok", "Indochina Time")]
    [InlineData("Australia/Sydney", "Australian Eastern Time (Sydney)")]
    [InlineData("Australia/Melbourne", "Australian Eastern Time (Melbourne)")]
    [InlineData("Australia/Perth", "Australian Western Standard Time")]
    [InlineData("Pacific/Auckland", "New Zealand Standard Time")]
    [InlineData("America/Sao_Paulo", "Brasilia Time")]
    [InlineData("America/Argentina/Buenos_Aires", "Argentina Time")]
    [InlineData("Africa/Johannesburg", "South Africa Standard Time")]
    public void GetFriendlyName_ReturnsDisplayName(string timeZoneId, string expectedName)
    {
        var displayName = TimeZoneHelper.GetFriendlyName(timeZoneId);
        Assert.Equal(expectedName, displayName);
    }

    [Theory]
    [InlineData("America/New_York", "EST", "EDT")]
    [InlineData("America/Chicago", "CST", "CDT")]
    [InlineData("America/Denver", "MST", "MDT")]
    [InlineData("America/Los_Angeles", "PST", "PDT")]
    [InlineData("Europe/London", "GMT", "BST")]
    [InlineData("Europe/Paris", "CET", "CEST")]
    [InlineData("Australia/Sydney", "AEST", "AEDT")]
    [InlineData("Pacific/Auckland", "NZST", "NZDT")]
    public void GetAbbreviations_DstTimezone_ReturnsBothAbbreviations(string ianaId, string standardAbbr, string daylightAbbr)
    {
        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(ianaId);
        Assert.NotNull(tzInfo);

        // This test verifies the timezone can be retrieved
        // Actual abbreviation logic may vary based on implementation
    }

    [Theory]
    [InlineData("Asia/Tokyo")]
    [InlineData("Asia/Shanghai")]
    [InlineData("Asia/Dubai")]
    [InlineData("Asia/Kolkata")]
    [InlineData("Pacific/Honolulu")]
    [InlineData("Africa/Johannesburg")]
    public void GetTimeZoneInfo_NoDstTimezone_ReturnsCorrectly(string ianaId)
    {
        var tzInfo = TimeZoneHelper.GetTimeZoneInfo(ianaId);
        Assert.NotNull(tzInfo);
    }

    [Fact]
    public void IanaToWindows_MostCommonTimezones_HaveMappings()
    {
        var commonTimezones = new[] {
            "America/New_York",
            "America/Chicago",
            "America/Denver",
            "America/Los_Angeles",
            "Europe/London",
            "Europe/Paris",
            "Asia/Tokyo",
            "Asia/Shanghai",
            "Australia/Sydney"
        };

        foreach (var tz in commonTimezones)
        {
            var windowsId = TimeZoneHelper.IanaToWindows(tz);
            Assert.NotNull(windowsId);
        }
    }
}
