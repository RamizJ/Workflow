using System;
using Workflow.VM.Common;

namespace Workflow.VM.ViewModels
{
    public class ProjectStatisticOptions
    {
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
    }
    
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
    }
}