using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PageLoading
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this T[] items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<int> SelectIntValues(this IEnumerable<object> values)
        {
            return values
                .Where(v => int.TryParse(v.ToString(), out _))
                .Select(v => int.Parse(v.ToString() ?? string.Empty, NumberStyles.Integer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> SelectDateTimeValues(this IEnumerable<object> values)
        {
            return values
                .Where(v => DateTime.TryParse(v.ToString(), out _))
                .Select(v => DateTime.Parse(v.ToString() ?? string.Empty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IEnumerable<bool> SelectBoolValues(this IEnumerable<object> values)
        {
            return values
                .Where(v => bool.TryParse(v.ToString(), out _))
                .Select(v => bool.Parse(v.ToString() ?? string.Empty));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> SelectEnumValues<T>(this IEnumerable<object> values)
            where T : struct, IConvertible
        {
            var result = values.Select(v =>
            {
                T? val = null;
                if (Enum.TryParse<T>(v.ToString(), out var s))
                    val = s;

                return val;
            }).Where(s => s != null).Cast<T>();

            return result;
        }
    }
}
