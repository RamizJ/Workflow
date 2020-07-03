using System;

namespace Workflow.VM.Common
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
        /// Значения поля
        /// </summary>
        public object[] Values { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public FieldFilter()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="values">Значение</param>
        public FieldFilter(string fieldName, object[] values)
        {
            FieldName = fieldName;
            Values = values;
        }


        /// <summary>
        /// Проверка на совпадение имени поля с передаваемым. Регистр букв не учитывается
        /// </summary>
        /// <param name="fieldName">Имя поля</param>
        /// <returns></returns>
        public bool SameAs(string fieldName)
        {
            return FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool HasValuesAndSameAs(string fieldName)
        {
            return SameAs(fieldName) && Values != null && Values.Length > 0;
        }
    }
}
