namespace TimeZoneKit.Tests;

public class SearchTests
{
    [Theory]
    [InlineData("eastern", "America/New_York")]
    [InlineData("pacific", "America/Los_Angeles")]
    [InlineData("tokyo", "Asia/Tokyo")]
    public void Search_FindsMatchingTimezones(string query, string expectedResult)
    {
        var results = TimeZoneKit.Search(query);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Contains(expectedResult, results);
    }

    [Fact]
    public void Search_CaseInsensitive_WorksCorrectly()
    {
        var results1 = TimeZoneKit.Search("EASTERN");
        var results2 = TimeZoneKit.Search("eastern");
        var results3 = TimeZoneKit.Search("Eastern");

        Assert.Equal(results1.Count, results2.Count);
        Assert.Equal(results2.Count, results3.Count);
    }

    [Fact]
    public void Search_EmptyQuery_ReturnsEmptyList()
    {
        var results = TimeZoneKit.Search("");
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void Search_NoMatches_ReturnsEmptyList()
    {
        var results = TimeZoneKit.Search("XyZabc123");
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public void Search_PartialMatch_FindsResults()
    {
        var results = TimeZoneKit.Search("amer");

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        // Should find America/New_York, America/Chicago, etc.
    }

    [Theory]
    [InlineData("asia", "Asia/Tokyo")]
    [InlineData("europe", "Europe/London")]
    [InlineData("pacific", "Pacific/Auckland")]
    [InlineData("australia", "Australia/Sydney")]
    [InlineData("america", "America/New_York")]
    public void Search_ContinentName_FindsMultipleResults(string continent, string expectedResult)
    {
        var results = TimeZoneKit.Search(continent);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Contains(expectedResult, results);
    }

    [Theory]
    [InlineData("york")]
    [InlineData("angeles")]
    [InlineData("francisco")]
    [InlineData("paulo")]
    [InlineData("kong")]
    public void Search_PartialCityName_FindsResults(string partial)
    {
        var results = TimeZoneKit.Search(partial);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

    [Theory]
    [InlineData("UTC")]
    [InlineData("GMT")]
    [InlineData("EST")]
    [InlineData("PST")]
    [InlineData("JST")]
    public void Search_AbbreviationSearch_FindsResults(string abbreviation)
    {
        var results = TimeZoneKit.Search(abbreviation);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("time")]
    public void Search_CommonWords_FindsMultipleResults(string word)
    {
        var results = TimeZoneKit.Search(word);

        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void GetCommonTimezones_ReturnsPopularZones()
    {
        var common = TimeZoneKit.GetCommonTimezones();

        Assert.NotNull(common);
        Assert.NotEmpty(common);
        Assert.Contains("America/New_York", common);
        Assert.Contains("Europe/London", common);
        Assert.Contains("Asia/Tokyo", common);
    }

    [Fact]
    public void GetCommonTimezones_ReturnsSameListOnMultipleCalls()
    {
        var common1 = TimeZoneKit.GetCommonTimezones();
        var common2 = TimeZoneKit.GetCommonTimezones();

        Assert.Equal(common1.Length, common2.Length);
    }
}
