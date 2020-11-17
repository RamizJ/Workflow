using System.Collections.Generic;
using System.Linq;

namespace PageLoading
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPageLoadService<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageOptions"></param>
        /// <returns></returns>
        public IQueryable<TModel> GetPage(IQueryable<TModel> query, 
            PageOptions pageOptions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IQueryable<TModel> Filter(IQueryable<TModel> query, 
            string filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterFields"></param>
        /// <returns></returns>
        public IQueryable<TModel> FilterByFields(IQueryable<TModel> query, 
            IEnumerable<FieldFilter> filterFields);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sortFields"></param>
        /// <returns></returns>
        public IQueryable<TModel> SortByFields(IQueryable<TModel> query,
            IEnumerable<FieldSort> sortFields);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQueryable<TModel> GetPageRows(IQueryable<TModel> query,
            int pageNumber, int pageSize);
    }
}
