using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PageLoading
{
    public static class QuerableExtension
    {
        public static IOrderedQueryable<T> SortBy<T, P>(this IQueryable<T> query,
            Expression<Func<T, P>> propertySelector, bool ascending)
        {
            if (query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                return ascending
                    ? query.OrderBy(propertySelector)
                    : query.OrderByDescending(propertySelector);
            }

            var orderedQuery = (IOrderedQueryable<T>)query;
            return ascending
                ? orderedQuery.ThenBy(propertySelector)
                : orderedQuery.ThenByDescending(propertySelector);
        }

        public static IOrderedQueryable<T> SortBy<T, P>(this IQueryable<T> query,
            Expression<Func<T, P>> propertySelector, bool ascending, IComparer<P> comparer)
        {
            if (query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                return ascending
                    ? query.OrderBy(propertySelector, comparer)
                    : query.OrderByDescending(propertySelector, comparer);
            }

            var orderedQuery = (IOrderedQueryable<T>)query;
            return ascending
                ? orderedQuery.ThenBy(propertySelector, comparer)
                : orderedQuery.ThenByDescending(propertySelector, comparer);
        }
    }
}