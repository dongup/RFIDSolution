using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Utils
{
    public static class AppExtensions
    {
        public static string GetDescription<T>(this T enumerationValue)
    where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static string RemoveUnicode(this string input)
        {
            if (input == null) return "";
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            return str2;
        }

        public static string ToLinuxPath(this string str)
        {
            return str.Replace('\\', '/');
        }

        /// <summary>
        /// Replace string with replacement value if it is null
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceNull(this string str, string replacement)
        {
            if (str == null) str = replacement;

            //Nếu str là null
            if (string.IsNullOrEmpty(str))
            {
                //Nếu có string thay thế thì mới thay thế
                if (!string.IsNullOrEmpty(replacement))
                {
                    str = replacement;
                }
            }

            return str;
        }

        /// <summary>
        /// Replace string with replacement value if it is null
        /// </summary>
        /// <param name="str"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceNull(this string str)
        {
            string replacement = "-";
            if (str == null) str = replacement;

            //Nếu str là null
            if (string.IsNullOrEmpty(str))
            {
                //Nếu có string thay thế thì mới thay thế
                if (!string.IsNullOrEmpty(replacement))
                {
                    str = replacement;
                }
            }

            return str;
        }

        /// <summary>
        /// Determine whether this string is null or empty
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string RelativePath(this string path)
        {
            if (path.Contains("wwwroot"))
            {
                String[] separator = { "wwwroot\\" };
                return path.Split(separator, StringSplitOptions.None)[1];
            }
            else
            {
                return path;
            }
        }

        private static CultureInfo vnCuture = new CultureInfo("vi-VN");

        public static string TimeAgo(this DateTime dateTime)
        {

            string result;

            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} giây trước", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    string.Format("Khoảng {0} phút trước", timeSpan.Minutes) : "Khoảng 1 phút trước";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    string.Format("Khoảng {0} giờ trước", timeSpan.Hours) : "Khoảng 1 giờ trước";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    string.Format("Khoảng {0} ngày trước", timeSpan.Days) : "Hôm qua";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    string.Format("Khoảng {0} tháng trước", timeSpan.Days / 30) : "Khoảng 1 tháng trước";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    string.Format("Khoảng {0} năm trước", timeSpan.Days / 365) : "Khoảng 1 năm trước";
            }

            return result;
        }

        /// <summary>
        /// Convert string qua datetime trả về ngày hiện tại nếu string null, quăng exception nếu string không đúng định dạng
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string date, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = vnCuture;
            }

            DateTime now = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParse(date, vnCuture, DateTimeStyles.None, out DateTime dOutDate))
                {
                    return dOutDate;
                }
                else
                {
                    throw new Exception($"Ngày tháng không đúng định dạng");
                }
            }
            else
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Convert ticks qua datetime trả về ngày hiện tại nếu string null, quăng exception nếu string không đúng định dạng
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long ticks)
        {
            var localTime = new DateTime(ticks, DateTimeKind.Utc);
            return localTime;
        }

        /// <summary>
        /// Convert ticks qua string giờ:phút:giây:milisec
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToTimeString(this long ticks)
        {
            var localTime = new DateTime(ticks, DateTimeKind.Utc);
            return localTime.ToString("HH:mm:ss:fff");
        }

        /// <summary>
        /// Convert ticks qua string giờ:phút ngày/tháng/năm
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this long ticks)
        {
            var localTime = new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
            return localTime.ToString("HH:mm dd/MM/yyyy");
        }


        /// <summary>
        /// Convert ticks qua string
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static string ToDateString(this long ticks)
        {
            var localTime = new DateTime(ticks, DateTimeKind.Utc).ToLocalTime();
            return localTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Convert string qua datetime trả về ngày hiện tại nếu string null, quăng exception nếu string không đúng định dạng
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static DateTime ToDateTimeExact(this string date)
        {
            DateTime now = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParseExact(date, "dd/MM/yyyy HH:mm", null, DateTimeStyles.None, out DateTime dOutDate))
                {
                    return dOutDate;
                }
                else
                {
                    throw new Exception($"Ngày tháng không đúng định dạng");
                }
            }
            else
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Convert string qua datetime trả về ngày hiện tại nếu string null, quăng exception nếu string không đúng định dạng
        /// </summary>
        /// <param name="date"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static DateTime ToSQLDateTime(this string date)
        {
            DateTime now = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime dOutDate))
                {
                    return dOutDate;
                }
                else
                {
                    throw new Exception($"Ngày tháng không đúng định dạng");
                }
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = (7 + (dt.DayOfWeek - DayOfWeek.Monday)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static bool Beetween(this DateTime date, string fromDate, string toDate)
        {
            if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
            {
                return true;
            }

            DateTime dFromDate = (DateTime)fromDate.ToDateTime();
            DateTime dToDate = (DateTime)toDate.ToDateTime();

            return date.Date >= dFromDate.Date && date.Date <= dToDate.Date;
        }

        public static bool Beetween(this DateTime date, DateTime fromDate, DateTime toDate)
        {
            return date >= fromDate && date <= toDate;
        }

        public static bool Like(this string str, string keyword)
        {
            keyword = keyword == null ? "" : keyword;
            keyword = keyword.ToLower();
            return str.RemoveUnicode().Trim().ToLower().Contains(keyword.RemoveUnicode().Trim())
                || keyword.IsNullOrEmpty();
        }

        public static List<T> Pagging<T>(this List<T> src, int pageItem, int pageIndex)
        {
            //int index = pageIndex - 1;
            int index = pageIndex;
            index = index < 0 ? 0 : index;

            int skip = index * pageItem;
            int take = pageItem;

            src = src.Skip(skip).Take(take).ToList();

            return src;
        }

        public static IQueryable<T> Pagging<T>(this IQueryable<T> src, int pageItem, int pageIndex)
        {
            //int index = pageIndex - 1;
            int index = pageIndex;
            index = index < 0 ? 0 : index;

            int skip = index * pageItem;
            int take = pageItem;

            src = src.Skip(skip).Take(take);

            return src;
        }

        public static IEnumerable<T> Pagging<T>(this IEnumerable<T> src, int pageItem, int pageIndex)
        {
            //int index = pageIndex - 1;
            int index = pageIndex;
            index = index < 0 ? 0 : index;

            int skip = index * pageItem;
            int take = pageItem;

            src = src.Skip(skip).Take(take);

            return src;
        }

        public static string ToSqlString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm");
        }

        public static string ToSqlDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToVNString(this DateTime date)
        {
            return date.ToString("HH:mm dd/MM/yyyy");
        }

        public static string ToShortVNString(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string ToShortVNString(this DateTime? date)
        {
            return date?.ToString("dd/MM/yyyy");
        }

        public static string ToVNString(this DateTime? date)
        {
            if (date == null) return "";
            return ((DateTime)date).ToString("HH:mm dd/MM/yyyy");
        }

        public static string JustifyBarCode(this string code)
        {
            return code.Replace("\"", "");
        }

        public static string RemoveTrail(this decimal d)
        {
            return d.ToString("G29");
        }
    }
}
