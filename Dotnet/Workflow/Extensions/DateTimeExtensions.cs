using System.Globalization;

namespace Workflow.Extensions;

public static class DateTimeExtensions
{
    public static int GetIso8601WeekOfYear(this DateTime time)
    {
        // ISO 8601 weeks start on Monday and the first week has at least four days.
        // Adjust the date to the nearest Thursday to correctly calculate the week number.
        var dayOfWeek = (int)CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);

        // Adjust to Thursday if the current day is Monday, Tuesday, or Wednesday.
        time = time.AddDays(3 - ((dayOfWeek + 6) % 7));
        // Return the ISO 8601 week number.
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
    }
}