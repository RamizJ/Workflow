using System;

namespace Workflow.Share
{
    public static class StringExtensions
    {
        public static bool EqualsNoCase(this string str1, string str2)
        {
            return str1.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EqualsNoCase(this string str1, object str2)
        {
            return str1.Equals(str2.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
