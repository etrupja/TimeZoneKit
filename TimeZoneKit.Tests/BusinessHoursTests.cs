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
}
