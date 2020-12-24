using System;
using System.Collections.Generic;
using System.Linq;

namespace PageLoading
{

    /// <inheritdoc />
    public abstract class PageLoadService<TModel> : IPageLoadService<TModel>
        where TModel : class, new()
    {
        /// <inheritdoc />
        public IQueryable<TModel> GetPage(IQueryable<TModel> query, 
            PageOptions pageOptions)
        {
            query = Filter(query, pageOptions.Filter);
            query = OnQueryFiltered(query);
            query = FilterByFields(query, pageOptions.FilterFields);
            query = OnQueryFilteredByFields(query);
            query = SortByFields(query, pageOptions.SortFields);
            query = OnQuerySortedByFields(query);
            query = GetPageRows(query, pageOptions.PageNumber, pageOptions.PageSize);
            query = OnQueryPageExtracted(query);

            return query;
        }

        /// <inheritdoc />
        public IQueryable<TModel> Filter(IQueryable<TModel> query, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return query;

            var words = filter.Split(" ");
            foreach (var word in words.Select(w => w.ToLower())) 
                query = FilterByWord(query, word);

            return query;
        }

        protected abstract IQueryable<TModel> FilterByWord(
            IQueryable<TModel> query,
            string word);

        /// <inheritdoc />
        public IQueryable<TModel> FilterByFields(
            IQueryable<TModel> query,
            IEnumerable<FieldFilter> filterFields)
        {
            if (filterFields == null) return query;

            foreach (var field in filterFields.Where(ff => ff != null)) 
                query = FilterByField(query, field);

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        protected abstract IQueryable<TModel> FilterByField(IQueryable<TModel> query, FieldFilter field);

        /// <inheritdoc />
        public IQueryable<TModel> SortByFields(
            IQueryable<TModel> query,
            IEnumerable<FieldSort> sortFields)
        {
            if (sortFields == null)
                return query;

            foreach (var field in sortFields.Where(f => f != null))
            {
                var isAscending = field.SortType == SortType.Ascending;
                query = SortByField(query, field, isAscending);
            }

            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="isAscending"></param>
        /// <returns></returns>
        protected abstract IQueryable<TModel> SortByField(IQueryable<TModel> query, 
            FieldSort field, bool isAscending);

        /// <inheritdoc />
        public IQueryable<TModel> GetPageRows(
            IQueryable<TModel> query, 
            int pageNumber, int pageSize)
        {
            return query.Skip(pageNumber * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="values"></param>
        /// <param name="querySelector"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <returns></returns>
        protected IQueryable<T> FilterByValues<T, TV>(IQueryable<T> query,
            IEnumerable<TV> values,
            Func<IQueryable<T>, TV, IQueryable<T>> querySelector)
        {
            var queries = values.Select(v => querySelector(query, v)).ToArray();
            if (queries.Any())
                query = queries.Aggregate(queries.First(), (current, q) => current.Union(q));

            return query;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQueryable<TModel> OnQueryFiltered(IQueryable<TModel> query)
        {
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQueryable<TModel> OnQuerySortedByFields(IQueryable<TModel> query)
        {
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQueryable<TModel> OnQueryFilteredByFields(IQueryable<TModel> query)
        {
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual IQueryable<TModel> OnQueryPageExtracted(IQueryable<TModel> query)
        {
            return query;
        }
    }
}