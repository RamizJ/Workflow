using System;
using System.Collections.Generic;

namespace Workflow.Share.Extensions
{
    public static class TreeExtension
    {
        public static void TraverseTree<T>(this IEnumerable<T> items, 
            Func<T, IEnumerable<T>> childrenSelector,
            Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
                TraverseTree(childrenSelector(item), childrenSelector, action);
            }
        }
    }
}
