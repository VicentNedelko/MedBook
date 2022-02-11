using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MedBook.Services.Visits
{
    public class VisitService
    {
        public static DateTime GetMonday()
        {
            Calendar visitCalendar = CultureInfo.InvariantCulture.Calendar;
            DateTime nowDate = DateTime.Now;
            var currentDay = visitCalendar.GetDayOfWeek(DateTime.Now);
            while(currentDay != DayOfWeek.Monday)
            {
                nowDate = nowDate.AddDays(-1);
                currentDay = visitCalendar.GetDayOfWeek(nowDate);
            }
            return nowDate;
        }

        public static DateTime GetSunday()
        {
            Calendar visitCalendar = CultureInfo.InvariantCulture.Calendar;
            DateTime nowDate = DateTime.Now;
            var currentDay = visitCalendar.GetDayOfWeek(DateTime.Now);
            while (currentDay != DayOfWeek.Sunday)
            {
                nowDate = nowDate.AddDays(1);
                currentDay = visitCalendar.GetDayOfWeek(nowDate);
            }
            return nowDate;
        }
    }
}
