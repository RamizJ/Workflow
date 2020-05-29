using System;

namespace WorkflowService.Common
{
    /// <summary>
    /// Сортировка по полю
    /// </summary>
    public class FieldSort
    {
        /// <summary>
        /// Имя поля
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Тип сортировки
        /// </summary>
        public SortType SortType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public FieldSort()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">Имя поля по которому будет осуществляться сортировка</param>
        /// <param name="sortType">Тип сортировки</param>
        public FieldSort(string fieldName, SortType sortType)
        {
            FieldName = fieldName;
            SortType = sortType;
        }


        /// <summary>
        /// Проверка на совпадение имени поля с передаваемым. Регистр букв не учитывается
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        public bool Is(string fieldName)
        {
            return FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}