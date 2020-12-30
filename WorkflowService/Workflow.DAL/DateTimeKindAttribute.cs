using System;
using System.Linq;
using System.Reflection;

namespace Workflow.DAL
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeKindAttribute : Attribute
    {
        public DateTimeKindAttribute(DateTimeKind kind)
        {
            Kind = kind;
        }

        public DateTimeKind Kind { get; }

        public static void Apply(object entity)
        {
            if (entity == null)
                return;

            var properties = entity.GetType().GetProperties()
                .Where(x => x.PropertyType == typeof(DateTime) || x.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<DateTimeKindAttribute>();
                if (attr == null)
                    continue;

                DateTime? dt;
                if (property.PropertyType == typeof(DateTime?))
                    dt = (DateTime?) property.GetValue(entity);
                else
                    dt = (DateTime) property.GetValue(entity);

                if (dt == null)
                    continue;

                property.SetValue(entity, DateTime.SpecifyKind(dt.Value, attr.Kind));
            }
        }
    }
}
