using System;
using System.Collections.Generic;
using System.Linq;

namespace Workflow.Share.EqualityComparers 
{
    public static class EqualityComparerExtension
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> inner, Func<T, T, bool> comparer)
        {
            return inner.Distinct(new LambdaComparer<T>(comparer));
        }
    }


    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        public LambdaComparer(Func<T, T, bool> compare)
        {
            _compare = compare;
        }

        public bool Equals(T x, T y)
        {
            return _compare(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }


        private readonly Func<T, T, bool> _compare;
    }
}
