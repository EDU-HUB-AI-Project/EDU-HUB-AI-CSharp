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

        /// <summary>
        /// 다양한 형식의 생년월일 입력을 YYMMDD 6자리로 정규화.
        /// 입력: YYYYMMDD(8자리) 또는 YYMMDD(6자리), 구분자(-/공백) 허용.
        /// 반환: YYMMDD 문자열, 유효하지 않으면 null.
        /// </summary>
        public static string? NormalizeBirthDate(string input)
        {
            var s = input.Trim().Replace("-", "").Replace(" ", "");

            int year, month, day;

            if (s.Length == 8 && s.All(char.IsDigit))
            {
                year = int.Parse(s.Substring(0, 4));
                month = int.Parse(s.Substring(4, 2));
                day = int.Parse(s.Substring(6, 2));
            }
            else if (s.Length == 6 && s.All(char.IsDigit))
            {
                var yy = int.Parse(s.Substring(0, 2));
                year = yy <= DateTime.Now.Year % 100 ? 2000 + yy : 1900 + yy;
                month = int.Parse(s.Substring(2, 2));
                day = int.Parse(s.Substring(4, 2));
            }
            else
            {
                return null;
            }

            if(month < 1 || month > 12 || day < 1 || day >DateTime.DaysInMonth(year, month))
            {
                return null;
            }

            return s.Length == 8 ? s.Substring(2) : s;
        }
    }
}
