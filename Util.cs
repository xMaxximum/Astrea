using DisCatSharp.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

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
                Console.WriteLine("{0} = {1}", name, value);
            }
        }

        public static async Task<DiscordAuditLogEntry> TryGetAuditLogEntry(DiscordGuild guild, AuditLogActionType type)
        {
            try
            {
                var audits = await guild.GetAuditLogsAsync(1, actionType: type);

                return (DiscordAuditLogEntry)(IReadOnlyList<DiscordAuditLogEntry>)audits.FirstOrDefault(e => DateTimeOffset.UtcNow - e.CreationTimestamp < TimeSpan.FromSeconds(30));
            }

            catch (Exception)
            {
                return null;
            }
        }
    }

    public static class Extensions
    {
        public static List<Variance> CompareRoleDifference(this DiscordRole role, DiscordRole second)
        {
            return role.Compare(second);
        }
    }

    public class Variance
    {
        public string PropertyName { get; set; }
        public object ValA { get; set; }
        public object ValB { get; set; }
    }

    public static class Comparision
    {
        public static List<Variance> Compare<T>(this T val1, T val2)
        {
            var variances = new List<Variance>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var v = new Variance
                {
                    PropertyName = property.Name,
                    ValA = property.GetValue(val1),
                    ValB = property.GetValue(val2)
                };
                if (v.ValA == null && v.ValB == null)
                {
                    continue;
                }
                if (
                    (v.ValA == null && v.ValB != null)
                    ||
                    (v.ValA != null && v.ValB == null)
                )
                {
                    variances.Add(v);
                    continue;
                }
                if (!v.ValA.Equals(v.ValB))
                {
                    variances.Add(v);
                }
            }
            return variances;
        }
    }
}
