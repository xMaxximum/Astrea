using System;
using System.ComponentModel;
using System.Globalization;

namespace BotName
{
    public class Util
    {
        public static DateTime ConvertStringToTime(string time, DateTime now)
        {
            DateTime until = now;
            string builder = "";
            foreach (char c in time)
            {
                switch (c)
                {
                    case 's':
                        until = until.AddSeconds(int.Parse(builder));
                        builder = "";
                        break;
                    case 'm':
                        until = until.AddMinutes(int.Parse(builder));
                        builder = "";
                        break;
                    case 'h':
                        until = until.AddHours(double.Parse(builder));
                        builder = "";
                        break;
                    case 'd':
                        until = until.AddDays(double.Parse(builder));
                        builder = "";
                        break;
                    case 'w':
                        until = until.AddDays(double.Parse(builder) * 7);
                        builder = "";
                        break;
                    default:
                        builder += c;
                        break;
                }
            }
            return until;
        }

        public static int WeekDiff(DateTime d1, DateTime d2, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = d2.Subtract(d1);

            var weeks = (int)diff.Days / 7;

            // need to check if there's an extra week to count
            var remainingDays = diff.Days % 7;
            var cal = CultureInfo.InvariantCulture.Calendar;
            var d1WeekNo = cal.GetWeekOfYear(d1, CalendarWeekRule.FirstFullWeek, startOfWeek);
            var d1PlusRemainingWeekNo = cal.GetWeekOfYear(d1.AddDays(remainingDays), CalendarWeekRule.FirstFullWeek, startOfWeek);

            if (d1WeekNo != d1PlusRemainingWeekNo)
                weeks++;

            return weeks;
        }

        public static string AddLint(string text) => "```ldif\n" + text + "```";

        public static void Print(object obj)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Console.WriteLine("{0}= {1}", name, value);
            }
        }
    }
}
