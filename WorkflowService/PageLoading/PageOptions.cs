using System.Linq;

namespace PageLoading
{
    /// <summary>
    /// Параметры загрузки страницы
    /// </summary>
    public class PageOptions
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Фильтр по всем полям
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Конкретные поля фильтрации
        /// </summary>
        public FieldFilter[] FilterFields { get; set; }

        /// <summary>
        /// Поля сортировки
        /// </summary>
        public FieldSort[] SortFields { get; set; }

        /// <summary>
        /// Загрука записей вместе с удаленными
        /// </summary>
        public bool WithRemoved { get; set; }


        public PageOptions()
        { }

        public PageOptions(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public bool HasFilterFields()
        {
            return FilterFields != null && FilterFields.Any();
        }

        public bool HasSortFields()
        {
            return SortFields != null && SortFields.Any();
        }
    }
}