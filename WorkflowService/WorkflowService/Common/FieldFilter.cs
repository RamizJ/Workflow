using System;

namespace WorkflowService.Common
{
    /// <summary>
    /// Фильтр по полю
    /// </summary>
    public class FieldFilter
    {
        /// <summary>
        /// Имя поля
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Значение поля
        /// </summary>
        public object Value { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public FieldFilter()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="value">Значение</param>
        public FieldFilter(string fieldName, object value)
        {
            FieldName = fieldName;
            Value = value;
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
