namespace EDU_HUB_AI.Config.Component.Common
{
    public static class DateHelper
    {
        /// <summary>해당 주 월요일 00:00 (일요일이면 이전 주 월요일)</summary>
        public static DateTime GetMonday(DateTime date)
        {
            var d = date.Date;
            var day = (int)d.DayOfWeek;
            var diff = day == 0 ? -6 : 1 - day;
            return d.AddDays(diff);
        }

        public static DateTime AddDays(DateTime date, int days) => date.Date.AddDays(days);

        public static string FormatIso(DateTime date) => date.ToString("yyyy-MM-dd");

        public static string FormatShort(DateTime date) => $"{date.Month}/{date.Day}";

        public static string FormatWeekRange(DateTime weekStart)
        {
            var end = weekStart.AddDays(4);
            return $"{weekStart:yyyy.MM.dd} ~ {end:MM.dd}";
        }
    }
}
