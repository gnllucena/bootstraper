using System.Collections.Generic;

namespace Console
{
    public static class Functions
    {
        public static string GetCamelCaseValue(string value)
        {
            return char.ToLower(value[0]) + value.Substring(1);
        }

        public static string GetConstantValue(List<string> constants, string value)
        {
            return constants.Find(x => x.ToLower() == value.ToLower());
        }
    }
}
