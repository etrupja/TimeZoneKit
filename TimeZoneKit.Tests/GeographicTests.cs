namespace TimeZoneKit.Tests;

public class GeographicTests
{
    [Theory]
    [InlineData("London", "Europe/London")]
    [InlineData("New York", "America/New_York")]
    [InlineData("Tokyo", "Asia/Tokyo")]
    [InlineData("Paris", "Europe/Paris")]
    [InlineData("Sydney", "Australia/Sydney")]
    public void FromCity_ReturnsCorrectTimeZone(string cityName, string expectedIanaId)
    {
        var tzInfo = TimeZoneKit.FromCity(cityName);
        Assert.NotNull(tzInfo);
        Assert.Equal(expectedIanaId, tzInfo.Id);
    }

    [Fact]
    public void FromCity_CaseInsensitive_WorksCorrectly()
    {
        var tz1 = TimeZoneKit.FromCity("London");
        var tz2 = TimeZoneKit.FromCity("london");
        var tz3 = TimeZoneKit.FromCity("LONDON");

        Assert.Equal(tz1.Id, tz2.Id);
        Assert.Equal(tz2.Id, tz3.Id);
    }

    [Fact]
    public void FromCity_InvalidCity_ThrowsTimeZoneNotFoundException()
    {
        Assert.Throws<TimeZoneNotFoundException>(() => TimeZoneKit.FromCity("InvalidCity"));
    }

    [Fact]
    public void FromCity_NullOrEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneKit.FromCity(null!));
        Assert.Throws<ArgumentException>(() => TimeZoneKit.FromCity(""));
    }

    [Theory]
    [InlineData("US", 6)] // At least 6 US timezones
    [InlineData("GB", 1)] // UK has 1 timezone
    [InlineData("JP", 1)] // Japan has 1 timezone
    [InlineData("AU", 2)] // Australia has multiple
    public void GetByCountry_ReturnsTimezones(string countryCode, int minExpectedCount)
    {
        var timezones = TimeZoneKit.GetByCountry(countryCode);
        Assert.NotNull(timezones);
        Assert.True(timezones.Length >= minExpectedCount);
    }

    [Fact]
    public void GetByCountry_CaseInsensitive_WorksCorrectly()
    {
        var tz1 = TimeZoneKit.GetByCountry("US");
        var tz2 = TimeZoneKit.GetByCountry("us");

        Assert.Equal(tz1.Length, tz2.Length);
    }

    [Fact]
    public void GetByCountry_InvalidCode_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneKit.GetByCountry("XX"));
    }

    [Fact]
    public void GetByCountry_NullOrEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneKit.GetByCountry(null!));
        Assert.Throws<ArgumentException>(() => TimeZoneKit.GetByCountry(""));
    }

    [Fact]
    public void GetByOffset_PositiveOffset_WorksCorrectly()
    {
        var offset = TimeSpan.FromHours(9);
        var timezones = TimeZoneKit.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("Asia/Tokyo", timezones);
    }
    
    [Fact]
    public void GetByOffset_ReturnsMatchingTimezones()
    {
        var offset = TimeSpan.FromHours(-5);
        var timezones = TimeZoneKit.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.NotEmpty(timezones);
        Assert.Contains("America/Bogota", timezones); // Always UTC-5, no DST
    }

    [Fact]
    public void GetByOffset_ZeroOffset_ReturnsGmtTimezones()
    {
        var offset = TimeSpan.Zero;
        var timezones = TimeZoneKit.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("UTC", timezones); // Always UTC+0
    }
}
