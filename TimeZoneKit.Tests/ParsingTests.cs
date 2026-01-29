namespace TimeZoneKit.Tests;

public class ParsingTests
{
    [Theory]
    [InlineData("EST", "America/New_York")]
    [InlineData("PST", "America/Los_Angeles")]
    [InlineData("GMT", "Europe/London")]
    [InlineData("JST", "Asia/Tokyo")]
    [InlineData("CST", "America/Chicago")]
    [InlineData("MST", "America/Denver")]
    [InlineData("AKST", "America/Anchorage")]
    [InlineData("HST", "Pacific/Honolulu")]
    [InlineData("CET", "Europe/Paris")]
    [InlineData("IST-IN", "Asia/Kolkata")]
    [InlineData("HKT", "Asia/Hong_Kong")]
    [InlineData("SGT", "Asia/Singapore")]
    [InlineData("AEST", "Australia/Sydney")]
    [InlineData("NZST", "Pacific/Auckland")]
    [InlineData("BRT", "America/Sao_Paulo")]
    [InlineData("SAST", "Africa/Johannesburg")]
    [InlineData("KST", "Asia/Seoul")]
    [InlineData("PKT", "Asia/Karachi")]
    [InlineData("NPT", "Asia/Kathmandu")]
    [InlineData("WIB", "Asia/Jakarta")]
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
    [InlineData("Toronto", "America/Toronto")]
    [InlineData("Vancouver", "America/Vancouver")]
    [InlineData("Sydney", "Australia/Sydney")]
    [InlineData("Melbourne", "Australia/Melbourne")]
    [InlineData("Dubai", "Asia/Dubai")]
    [InlineData("Singapore", "Asia/Singapore")]
    [InlineData("Hong Kong", "Asia/Hong_Kong")]
    [InlineData("Shanghai", "Asia/Shanghai")]
    [InlineData("Beijing", "Asia/Shanghai")]
    [InlineData("Seoul", "Asia/Seoul")]
    [InlineData("Mumbai", "Asia/Kolkata")]
    [InlineData("Delhi", "Asia/Kolkata")]
    [InlineData("Bangkok", "Asia/Bangkok")]
    [InlineData("Istanbul", "Europe/Istanbul")]
    [InlineData("Moscow", "Europe/Moscow")]
    [InlineData("Mexico City", "America/Mexico_City")]
    [InlineData("Sao Paulo", "America/Sao_Paulo")]
    [InlineData("Buenos Aires", "America/Argentina/Buenos_Aires")]
    [InlineData("Cairo", "Africa/Cairo")]
    [InlineData("Johannesburg", "Africa/Johannesburg")]
    [InlineData("Auckland", "Pacific/Auckland")]
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
    [InlineData("America/Chicago")]
    [InlineData("America/Denver")]
    [InlineData("America/Los_Angeles")]
    [InlineData("America/Phoenix")]
    [InlineData("America/Anchorage")]
    [InlineData("Pacific/Honolulu")]
    [InlineData("America/Toronto")]
    [InlineData("America/Vancouver")]
    [InlineData("America/Halifax")]
    [InlineData("America/St_Johns")]
    [InlineData("America/Mexico_City")]
    [InlineData("Europe/Paris")]
    [InlineData("Europe/Berlin")]
    [InlineData("Europe/Madrid")]
    [InlineData("Europe/Rome")]
    [InlineData("Europe/Amsterdam")]
    [InlineData("Europe/Athens")]
    [InlineData("Europe/Istanbul")]
    [InlineData("Europe/Moscow")]
    [InlineData("Asia/Dubai")]
    [InlineData("Asia/Kolkata")]
    [InlineData("Asia/Bangkok")]
    [InlineData("Asia/Singapore")]
    [InlineData("Asia/Hong_Kong")]
    [InlineData("Asia/Shanghai")]
    [InlineData("Asia/Seoul")]
    [InlineData("Australia/Sydney")]
    [InlineData("Australia/Melbourne")]
    [InlineData("Australia/Perth")]
    [InlineData("Pacific/Auckland")]
    [InlineData("America/Sao_Paulo")]
    [InlineData("America/Argentina/Buenos_Aires")]
    [InlineData("Africa/Johannesburg")]
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
    [InlineData("Central Standard Time")]
    [InlineData("Mountain Standard Time")]
    [InlineData("Alaskan Standard Time")]
    [InlineData("Hawaiian Standard Time")]
    [InlineData("Romance Standard Time")]
    [InlineData("W. Europe Standard Time")]
    [InlineData("Tokyo Standard Time")]
    [InlineData("China Standard Time")]
    [InlineData("India Standard Time")]
    [InlineData("AUS Eastern Standard Time")]
    [InlineData("New Zealand Standard Time")]
    [InlineData("Arabian Standard Time")]
    [InlineData("Russian Standard Time")]
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
