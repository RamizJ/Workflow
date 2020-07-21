using System;
using System.Linq.Expressions;

namespace Workflow.Share.Extensions
{
    public static class TypeExtension
    {
        public static string GetPropertyFullName<T, P>(Expression<Func<T, P>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    me = (expr.Body is UnaryExpression ue ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = expr.Body as MemberExpression;
                    break;
            }

            var propName = string.Empty;
            for (int i = 0; me != null; i++)
            {
                propName = propName.Insert(0, i == 0 ? me.Member.Name : $"{me.Member.Name}.");
                me = me.Expression as MemberExpression;
            }

            return propName;
        }
    }
}
