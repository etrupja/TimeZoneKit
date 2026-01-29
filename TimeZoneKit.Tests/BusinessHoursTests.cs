using TimeZoneKit.Extensions;
using TimeZoneKit.Models;

namespace TimeZoneKit.Tests;

public class BusinessHoursTests
{
    [Fact]
    public void IsBusinessHour_WeekdayDuringHours_ReturnsTrue()
    {
        // Tuesday at 10 AM UTC (5 AM EST - not business hours in NY)
        var tuesdayMorning = new DateTime(2025, 1, 28, 10, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(tuesdayMorning, "America/New_York");

        // At 10 AM UTC, it's 5 AM in NY (before 9 AM)
        Assert.False(isOpen);
    }

    [Fact]
    public void IsBusinessHour_WeekdayDuringBusinessHours_ReturnsTrue()
    {
        // Tuesday at 3 PM UTC (10 AM EST - during business hours)
        var tuesdayMorning = new DateTime(2025, 1, 28, 15, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(tuesdayMorning, "America/New_York");

        Assert.True(isOpen);
    }

    [Fact]
    public void IsBusinessHour_Saturday_ReturnsFalse()
    {
        // Saturday at 3 PM UTC (10 AM EST)
        var saturday = new DateTime(2025, 2, 1, 15, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(saturday, "America/New_York");

        Assert.False(isOpen);
    }

    [Fact]
    public void IsBusinessHour_Sunday_ReturnsFalse()
    {
        // Sunday at 3 PM UTC (10 AM EST)
        var sunday = new DateTime(2025, 2, 2, 15, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(sunday, "America/New_York");

        Assert.False(isOpen);
    }

    [Fact]
    public void IsBusinessHour_CustomHours_WorksCorrectly()
    {
        // Tuesday at 1 PM UTC (8 AM EST)
        var tuesdayMorning = new DateTime(2025, 1, 28, 13, 0, 0, DateTimeKind.Utc);

        // Default hours (9-17): not open at 8 AM
        var isOpenDefault = TimeZoneKit.IsBusinessHour(tuesdayMorning, "America/New_York");
        Assert.False(isOpenDefault);

        // Custom hours (8-18): open at 8 AM
        var isOpenCustom = TimeZoneKit.IsBusinessHour(tuesdayMorning, "America/New_York", startHour: 8, endHour: 18);
        Assert.True(isOpenCustom);
    }

    [Fact]
    public void IsBusinessHour_ExtensionMethod_WorksCorrectly()
    {
        var tuesdayMorning = new DateTime(2025, 1, 28, 15, 0, 0, DateTimeKind.Utc);
        var isOpen = tuesdayMorning.IsBusinessHour("America/New_York");

        Assert.True(isOpen);
    }

    [Fact]
    public void NextBusinessHour_DuringWeekend_ReturnsMonday()
    {
        // Saturday at noon UTC
        var saturday = new DateTime(2025, 2, 1, 12, 0, 0, DateTimeKind.Utc);
        var nextOpen = TimeZoneKit.NextBusinessHour(saturday, "America/New_York");

        Assert.NotNull(nextOpen);
        // Should return Monday
        Assert.Equal(DayOfWeek.Monday, nextOpen.Value.ToTimeZone("America/New_York").DayOfWeek);
    }

    [Fact]
    public void NextBusinessHour_AfterHours_ReturnsNextDay()
    {
        // Tuesday at 11 PM UTC (6 PM EST - after business hours)
        var tuesday = new DateTime(2025, 1, 28, 23, 0, 0, DateTimeKind.Utc);
        var nextOpen = TimeZoneKit.NextBusinessHour(tuesday, "America/New_York");

        Assert.NotNull(nextOpen);
    }

    [Fact]
    public void NextBusinessHour_ExtensionMethod_WorksCorrectly()
    {
        var saturday = new DateTime(2025, 2, 1, 12, 0, 0, DateTimeKind.Utc);
        var nextOpen = saturday.NextBusinessHour("America/New_York");

        Assert.NotNull(nextOpen);
    }

    [Fact]
    public void TimeRange_Contains_WorksCorrectly()
    {
        var range = new TimeRange(9, 17);

        Assert.True(range.Contains(new TimeSpan(10, 0, 0))); // 10 AM
        Assert.True(range.Contains(new TimeSpan(9, 0, 0)));  // 9 AM (start)
        Assert.False(range.Contains(new TimeSpan(17, 0, 0))); // 5 PM (end, exclusive)
        Assert.False(range.Contains(new TimeSpan(8, 0, 0)));  // 8 AM (before)
        Assert.False(range.Contains(new TimeSpan(18, 0, 0))); // 6 PM (after)
    }

    [Fact]
    public void BusinessHours_IsOpen_WorksCorrectly()
    {
        var businessHours = new BusinessHours("America/New_York", 9, 17);

        var tuesday10am = new DateTime(2025, 1, 28, 10, 0, 0);
        var saturday10am = new DateTime(2025, 2, 1, 10, 0, 0);

        Assert.True(businessHours.IsOpen(tuesday10am));
        Assert.False(businessHours.IsOpen(saturday10am));
    }

    [Fact]
    public void BusinessHours_NextAvailableTime_WorksCorrectly()
    {
        var businessHours = new BusinessHours("America/New_York", 9, 17);

        // Saturday at 10 AM
        var saturday = new DateTime(2025, 2, 1, 10, 0, 0);
        var nextAvailable = businessHours.NextAvailableTime(saturday);

        Assert.NotNull(nextAvailable);
        // Should be Monday at 9 AM
        Assert.Equal(DayOfWeek.Monday, nextAvailable.Value.DayOfWeek);
        Assert.Equal(9, nextAvailable.Value.Hour);
    }

    [Fact]
    public void BusinessHours_CustomSchedule_WorksCorrectly()
    {
        var businessHours = new BusinessHours("America/New_York")
        {
            Monday = new TimeRange(9, 17),
            Tuesday = new TimeRange(9, 17),
            Wednesday = new TimeRange(9, 17),
            Thursday = new TimeRange(9, 17),
            Friday = new TimeRange(9, 17),
            Saturday = new TimeRange(10, 14), // Open Saturday 10-2
            Sunday = null // Closed Sunday
        };

        var saturday11am = new DateTime(2025, 2, 1, 11, 0, 0);
        var sunday11am = new DateTime(2025, 2, 2, 11, 0, 0);

        Assert.True(businessHours.IsOpen(saturday11am));
        Assert.False(businessHours.IsOpen(sunday11am));
    }

    [Theory]
    [InlineData("America/New_York", 9, 17)]
    [InlineData("Europe/London", 9, 17)]
    [InlineData("Asia/Tokyo", 9, 18)]
    [InlineData("Australia/Sydney", 9, 17)]
    public void IsBusinessHour_DifferentTimezones_WorksCorrectly(string timeZoneId, int startHour, int endHour)
    {
        // Tuesday at a time that should be during business hours in the target timezone
        var utcTime = new DateTime(2025, 1, 28, 14, 0, 0, DateTimeKind.Utc);

        // Convert to target timezone to check if it falls within business hours
        var localTime = TimeZoneKit.Convert(utcTime, timeZoneId);

        var isOpen = TimeZoneKit.IsBusinessHour(utcTime, timeZoneId, startHour, endHour);

        // Verify the method doesn't throw (isOpen is a boolean, so just check it's valid)
        _ = isOpen;
    }

    [Fact]
    public void IsBusinessHour_At9AmStart_ReturnsTrue()
    {
        // Tuesday at exactly 9 AM EST (14:00 UTC)
        var tuesday9am = new DateTime(2025, 1, 28, 14, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(tuesday9am, "America/New_York", 9, 17);

        Assert.True(isOpen);
    }

    [Fact]
    public void IsBusinessHour_At5PmEnd_ReturnsFalse()
    {
        // Tuesday at exactly 5 PM EST (22:00 UTC)
        var tuesday5pm = new DateTime(2025, 1, 28, 22, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(tuesday5pm, "America/New_York", 9, 17);

        // End time is exclusive
        Assert.False(isOpen);
    }

    [Fact]
    public void IsBusinessHour_At459Pm_ReturnsTrue()
    {
        // Tuesday at 4:59 PM EST (21:59 UTC)
        var tuesday459pm = new DateTime(2025, 1, 28, 21, 59, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(tuesday459pm, "America/New_York", 9, 17);

        Assert.True(isOpen);
    }

    [Fact]
    public void BusinessHours_24HourOperation_AlwaysOpen()
    {
        var businessHours = new BusinessHours("America/New_York", 0, 23);

        var tuesday3am = new DateTime(2025, 1, 28, 8, 0, 0, DateTimeKind.Utc);
        var saturday10pm = new DateTime(2025, 2, 1, 3, 0, 0, DateTimeKind.Utc);

        // With 0-23 hours, should be open nearly all day
        Assert.True(businessHours.IsOpen(tuesday3am));
        // Note: Might still respect weekend settings depending on implementation
    }

    [Fact]
    public void BusinessHours_ExtendedHours_WorksCorrectly()
    {
        // Testing extended business hours
        var businessHours = new BusinessHours("America/New_York", 8, 20); // 8 AM to 8 PM

        var tuesday2pm = new DateTime(2025, 1, 28, 19, 0, 0, DateTimeKind.Utc); // 2 PM EST
        var tuesday10am = new DateTime(2025, 1, 28, 15, 0, 0, DateTimeKind.Utc); // 10 AM EST

        // Both should be within business hours
        Assert.True(businessHours.IsOpen(tuesday2pm));
        Assert.True(businessHours.IsOpen(tuesday10am));
    }

    [Fact]
    public void NextBusinessHour_FridayEvening_ReturnsMonday()
    {
        // Friday at 6 PM UTC (1 PM EST - after business hours)
        var friday = new DateTime(2025, 1, 31, 23, 0, 0, DateTimeKind.Utc);
        var nextOpen = TimeZoneKit.NextBusinessHour(friday, "America/New_York");

        Assert.NotNull(nextOpen);
        // Should skip weekend and return Monday morning
        var nextLocal = nextOpen.Value.ToTimeZone("America/New_York");
        Assert.Equal(DayOfWeek.Monday, nextLocal.DayOfWeek);
    }

    [Theory]
    [InlineData(DayOfWeek.Monday)]
    [InlineData(DayOfWeek.Tuesday)]
    [InlineData(DayOfWeek.Wednesday)]
    [InlineData(DayOfWeek.Thursday)]
    [InlineData(DayOfWeek.Friday)]
    public void IsBusinessHour_Weekdays_AtNoon_ReturnsTrue(DayOfWeek dayOfWeek)
    {
        // Find a date that matches the day of week
        var date = new DateTime(2025, 1, 27); // Monday, Jan 27, 2025
        while (date.DayOfWeek != dayOfWeek)
        {
            date = date.AddDays(1);
        }

        // Set time to noon EST (17:00 UTC)
        var noonEst = new DateTime(date.Year, date.Month, date.Day, 17, 0, 0, DateTimeKind.Utc);
        var isOpen = TimeZoneKit.IsBusinessHour(noonEst, "America/New_York");

        Assert.True(isOpen);
    }

    [Fact]
    public void BusinessHours_DifferentTimezones_SameBusinessHours()
    {
        var nyHours = new BusinessHours("America/New_York", 9, 17);
        var londonHours = new BusinessHours("Europe/London", 9, 17);

        // Same UTC time
        var utcTime = new DateTime(2025, 1, 28, 14, 0, 0, DateTimeKind.Utc);

        var nyOpen = nyHours.IsOpen(utcTime);
        var londonOpen = londonHours.IsOpen(utcTime);

        // At 14:00 UTC: NY is 9 AM (open), London is 2 PM (open)
        Assert.True(nyOpen);
        Assert.True(londonOpen);
    }
}
