using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Workflow.Share.Extensions
{
    public static class QuerableExtension
    {
        public static IOrderedQueryable<T> SortBy<T, P>(this IQueryable<T> query, 
            Expression<Func<T, P>> propertySelector, bool ascending)
        {
            if(query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                return ascending
                    ? query.OrderBy(propertySelector)
                    : query.OrderByDescending(propertySelector);
            }

            var orderedQuery = (IOrderedQueryable<T>) query;
            return ascending
                ? orderedQuery.ThenBy(propertySelector)
                : orderedQuery.ThenByDescending(propertySelector);
        }

        public static IOrderedQueryable<T> OrderByProperty<T>(this IQueryable<T> query, string propertyName, bool ascending)
        {
            var bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;
            if (typeof(T).GetProperty(propertyName, bindingFlags) == null)
                throw new InvalidOperationException("Cannot sort by property. Property not found");

            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);

            var genericMethod = ascending
                ? OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type)
                : OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

            var result = genericMethod.Invoke(null, new object[] {query, lambda});
            return result as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> ThenByProperty<T>(this IOrderedQueryable<T> query, string propertyName, bool ascending)
        {
            var bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;
            if (typeof(T).GetProperty(propertyName, bindingFlags) == null)
                throw new InvalidOperationException("Cannot sort by property. Property not found");

            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);

            var genericMethod = ascending
                ? ThenByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type)
                : ThenByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);

            var result = genericMethod.Invoke(null, new object[] {query, lambda});
            return result as IOrderedQueryable<T>;
        }


        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "OrderBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "ThenBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "ThenByDescending" && method.GetParameters().Length == 2);
    }
}
