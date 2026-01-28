namespace TimeZoneKit.Tests;

public class ParsingTests
{
    [Theory]
    [InlineData("EST", "America/New_York")]
    [InlineData("PST", "America/Los_Angeles")]
    [InlineData("GMT", "Europe/London")]
    [InlineData("JST", "Asia/Tokyo")]
    public void Parse_Abbreviation_ReturnsCorrectTimeZone(string abbreviation, string expectedIanaId)
    {
        var tz = TimeZoneKit.Parse(abbreviation);
        Assert.NotNull(tz);
        Assert.Contains(expectedIanaId, tz.Id);
    }

    [Theory]
    [InlineData("New York", "America/New_York")]
    [InlineData("London", "Europe/London")]
    [InlineData("Tokyo", "Asia/Tokyo")]
    [InlineData("Chicago", "America/Chicago")]
    public void Parse_CityName_ReturnsCorrectTimeZone(string cityName, string expectedIanaId)
    {
        var tz = TimeZoneKit.Parse(cityName);
        Assert.NotNull(tz);
        Assert.Equal(expectedIanaId, tz.Id);
    }

    [Theory]
    [InlineData("America/New_York")]
    [InlineData("Europe/London")]
    [InlineData("Asia/Tokyo")]
    public void Parse_IanaId_ReturnsCorrectTimeZone(string ianaId)
    {
        var tz = TimeZoneKit.Parse(ianaId);
        Assert.NotNull(tz);
        Assert.Equal(ianaId, tz.Id);
    }

    [Theory]
    [InlineData("Eastern Standard Time")]
    [InlineData("Pacific Standard Time")]
    [InlineData("GMT Standard Time")]
    public void Parse_WindowsId_ReturnsCorrectTimeZone(string windowsId)
    {
        var tz = TimeZoneKit.Parse(windowsId);
        Assert.NotNull(tz);
    }

    [Fact]
    public void Parse_InvalidTimeZone_ThrowsTimeZoneNotFoundException()
    {
        Assert.Throws<TimeZoneNotFoundException>(() => TimeZoneKit.Parse("InvalidZone"));
    }

    [Fact]
    public void Parse_NullOrEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneKit.Parse(null!));
        Assert.Throws<ArgumentException>(() => TimeZoneKit.Parse(""));
        Assert.Throws<ArgumentException>(() => TimeZoneKit.Parse("   "));
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        var result = TimeZoneKit.TryParse("EST", out var tzInfo);

        Assert.True(result);
        Assert.NotNull(tzInfo);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        var result = TimeZoneKit.TryParse("InvalidZone", out var tzInfo);

        Assert.False(result);
        Assert.Null(tzInfo);
    }

    [Theory]
    [InlineData("GMT-5")]
    [InlineData("UTC+9")]
    [InlineData("GMT+0")]
    public void Parse_OffsetString_ReturnsTimeZone(string offset)
    {
        var tz = TimeZoneKit.Parse(offset);
        Assert.NotNull(tz);
    }

    [Fact]
    public void Parse_CaseInsensitive_WorksCorrectly()
    {
        var tz1 = TimeZoneKit.Parse("EST");
        var tz2 = TimeZoneKit.Parse("est");
        var tz3 = TimeZoneKit.Parse("Est");

        Assert.NotNull(tz1);
        Assert.NotNull(tz2);
        Assert.NotNull(tz3);
    }
}
