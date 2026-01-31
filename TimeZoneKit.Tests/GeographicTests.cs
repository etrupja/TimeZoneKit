using TimeZoneKit.Methods;

namespace TimeZoneKit.Tests;

public class GeographicTests
{
    [Theory]
    [InlineData("London", "Europe/London")]
    [InlineData("New York", "America/New_York")]
    [InlineData("Tokyo", "Asia/Tokyo")]
    [InlineData("Paris", "Europe/Paris")]
    [InlineData("Sydney", "Australia/Sydney")]
    [InlineData("Toronto", "America/Toronto")]
    [InlineData("Vancouver", "America/Vancouver")]
    [InlineData("Montreal", "America/Montreal")]
    [InlineData("Calgary", "America/Edmonton")]
    [InlineData("Halifax", "America/Halifax")]
    [InlineData("Mexico City", "America/Mexico_City")]
    [InlineData("Dubai", "Asia/Dubai")]
    [InlineData("Singapore", "Asia/Singapore")]
    [InlineData("Hong Kong", "Asia/Hong_Kong")]
    [InlineData("Bangkok", "Asia/Bangkok")]
    [InlineData("Mumbai", "Asia/Kolkata")]
    [InlineData("Delhi", "Asia/Kolkata")]
    [InlineData("Bangalore", "Asia/Kolkata")]
    [InlineData("Seoul", "Asia/Seoul")]
    [InlineData("Shanghai", "Asia/Shanghai")]
    [InlineData("Beijing", "Asia/Shanghai")]
    [InlineData("Melbourne", "Australia/Melbourne")]
    [InlineData("Brisbane", "Australia/Brisbane")]
    [InlineData("Perth", "Australia/Perth")]
    [InlineData("Auckland", "Pacific/Auckland")]
    [InlineData("Wellington", "Pacific/Auckland")]
    [InlineData("Sao Paulo", "America/Sao_Paulo")]
    [InlineData("Rio de Janeiro", "America/Sao_Paulo")]
    [InlineData("Buenos Aires", "America/Argentina/Buenos_Aires")]
    [InlineData("Lima", "America/Lima")]
    [InlineData("Bogota", "America/Bogota")]
    [InlineData("Santiago", "America/Santiago")]
    [InlineData("Caracas", "America/Caracas")]
    [InlineData("Cairo", "Africa/Cairo")]
    [InlineData("Johannesburg", "Africa/Johannesburg")]
    [InlineData("Lagos", "Africa/Lagos")]
    [InlineData("Nairobi", "Africa/Nairobi")]
    [InlineData("Istanbul", "Europe/Istanbul")]
    [InlineData("Moscow", "Europe/Moscow")]
    [InlineData("Athens", "Europe/Athens")]
    [InlineData("Rome", "Europe/Rome")]
    [InlineData("Madrid", "Europe/Madrid")]
    [InlineData("Barcelona", "Europe/Madrid")]
    [InlineData("Berlin", "Europe/Berlin")]
    [InlineData("Munich", "Europe/Berlin")]
    [InlineData("Amsterdam", "Europe/Amsterdam")]
    [InlineData("Brussels", "Europe/Brussels")]
    [InlineData("Zurich", "Europe/Zurich")]
    [InlineData("Vienna", "Europe/Vienna")]
    [InlineData("Warsaw", "Europe/Warsaw")]
    [InlineData("Prague", "Europe/Prague")]
    [InlineData("Stockholm", "Europe/Stockholm")]
    [InlineData("Copenhagen", "Europe/Copenhagen")]
    [InlineData("Oslo", "Europe/Oslo")]
    [InlineData("Helsinki", "Europe/Helsinki")]
    public void FromCity_ReturnsCorrectTimeZone(string cityName, string expectedIanaId)
    {
        var tzInfo = TimeZoneHelper.FromCity(cityName);
        Assert.NotNull(tzInfo);
        Assert.Equal(expectedIanaId, tzInfo.Id);
    }

    [Fact]
    public void FromCity_CaseInsensitive_WorksCorrectly()
    {
        var tz1 = TimeZoneHelper.FromCity("London");
        var tz2 = TimeZoneHelper.FromCity("london");
        var tz3 = TimeZoneHelper.FromCity("LONDON");

        Assert.Equal(tz1.Id, tz2.Id);
        Assert.Equal(tz2.Id, tz3.Id);
    }

    [Fact]
    public void FromCity_InvalidCity_ThrowsTimeZoneNotFoundException()
    {
        Assert.Throws<TimeZoneNotFoundException>(() => TimeZoneHelper.FromCity("InvalidCity"));
    }

    [Fact]
    public void FromCity_NullOrEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneHelper.FromCity(null!));
        Assert.Throws<ArgumentException>(() => TimeZoneHelper.FromCity(""));
    }

    [Theory]
    [InlineData("US", 6)] // At least 6 US timezones
    [InlineData("GB", 1)] // UK has 1 timezone
    [InlineData("JP", 1)] // Japan has 1 timezone
    [InlineData("AU", 2)] // Australia has multiple
    [InlineData("CA", 5)] // Canada has multiple timezones
    [InlineData("MX", 3)] // Mexico has multiple timezones
    [InlineData("BR", 5)] // Brazil has multiple timezones
    [InlineData("AR", 3)] // Argentina has multiple timezones
    [InlineData("RU", 8)] // Russia has many timezones
    [InlineData("ID", 3)] // Indonesia has multiple timezones
    [InlineData("FR", 1)] // France has 1 main timezone
    [InlineData("DE", 1)] // Germany has 1 timezone
    [InlineData("IT", 1)] // Italy has 1 timezone
    [InlineData("ES", 1)] // Spain has at least 1 timezone
    [InlineData("IN", 1)] // India has 1 timezone
    [InlineData("CN", 1)] // China has 1 main timezone
    [InlineData("NZ", 1)] // New Zealand has at least 1 timezone
    public void GetByCountry_ReturnsTimezones(string countryCode, int minExpectedCount)
    {
        var timezones = TimeZoneHelper.GetByCountry(countryCode);
        Assert.NotNull(timezones);
        Assert.True(timezones.Length >= minExpectedCount);
    }

    [Fact]
    public void GetByCountry_CaseInsensitive_WorksCorrectly()
    {
        var tz1 = TimeZoneHelper.GetByCountry("US");
        var tz2 = TimeZoneHelper.GetByCountry("us");

        Assert.Equal(tz1.Length, tz2.Length);
    }

    [Fact]
    public void GetByCountry_InvalidCode_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneHelper.GetByCountry("XX"));
    }

    [Fact]
    public void GetByCountry_NullOrEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TimeZoneHelper.GetByCountry(null!));
        Assert.Throws<ArgumentException>(() => TimeZoneHelper.GetByCountry(""));
    }

    [Fact]
    public void GetByOffset_PositiveOffset_WorksCorrectly()
    {
        var offset = TimeSpan.FromHours(9);
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("Asia/Tokyo", timezones);
    }
    
    [Fact]
    public void GetByOffset_ReturnsMatchingTimezones()
    {
        var offset = TimeSpan.FromHours(-5);
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.NotEmpty(timezones);
        Assert.Contains("America/Bogota", timezones); // Always UTC-5, no DST
    }

    [Fact]
    public void GetByOffset_ZeroOffset_ReturnsGmtTimezones()
    {
        var offset = TimeSpan.Zero;
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("UTC", timezones); // Always UTC+0
    }

    [Theory]
    [InlineData(-8, "America/Los_Angeles")] // PST
    [InlineData(-7, "America/Phoenix")] // MST (no DST)
    [InlineData(-6, "America/Regina")] // CST (no DST)
    [InlineData(-5, "America/Bogota")] // COT (no DST)
    [InlineData(-4, "America/La_Paz")] // BOT (no DST)
    [InlineData(-3, "America/Argentina/Buenos_Aires")] // ART (no DST)
    [InlineData(1, "Europe/Paris")] // CET
    [InlineData(2, "Africa/Johannesburg")] // SAST (no DST)
    [InlineData(3, "Asia/Riyadh")] // AST (no DST)
    [InlineData(4, "Asia/Dubai")] // GST (no DST)
    [InlineData(5, "Asia/Karachi")] // PKT (no DST)
    [InlineData(5.5, "Asia/Kolkata")] // IST (no DST)
    [InlineData(5.75, "Asia/Kathmandu")] // NPT (no DST)
    [InlineData(6, "Asia/Dhaka")] // BST (no DST)
    [InlineData(7, "Asia/Bangkok")] // ICT (no DST)
    [InlineData(8, "Asia/Singapore")] // SGT (no DST)
    [InlineData(9, "Asia/Tokyo")] // JST (no DST)
    [InlineData(9.5, "Australia/Darwin")] // ACST (no DST)
    [InlineData(10, "Australia/Brisbane")] // AEST (no DST)
    [InlineData(12, "Pacific/Fiji")] // FJT
    public void GetByOffset_VariousOffsets_ReturnsExpectedTimezones(double hours, string expectedTimezone)
    {
        var offset = TimeSpan.FromHours(hours);
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains(expectedTimezone, timezones);
    }

    [Fact]
    public void GetByOffset_HalfHourOffset_WorksCorrectly()
    {
        // India Standard Time is UTC+5:30
        var offset = TimeSpan.FromMinutes(330); // 5.5 hours
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("Asia/Kolkata", timezones);
    }

    [Fact]
    public void GetByOffset_QuarterHourOffset_WorksCorrectly()
    {
        // Nepal Time is UTC+5:45
        var offset = TimeSpan.FromMinutes(345); // 5.75 hours
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("Asia/Kathmandu", timezones);
    }

    [Fact]
    public void GetByOffset_NewfoundlandOffset_WorksCorrectly()
    {
        // Newfoundland Standard Time is UTC-3:30
        var offset = TimeSpan.FromMinutes(-210); // -3.5 hours
        var timezones = TimeZoneHelper.GetByOffset(offset);

        Assert.NotNull(timezones);
        Assert.Contains("America/St_Johns", timezones);
    }
}
