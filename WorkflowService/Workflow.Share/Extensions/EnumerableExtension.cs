using System;
using System.Collections.Generic;

namespace Workflow.Share.Extensions
{
    public static class EnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items) 
                action(item);
        }

        public static void ForEach<T>(this T[] items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }
    }
}
