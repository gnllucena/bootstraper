using System.Collections.Generic;

namespace Console
{
    public static class Constants
    {
        public static readonly List<string> ProjectDatabases = new List<string>
        {
            "Mysql"
        };

        public static readonly List<string> DependsWhens = new List<string>
        {
            "GreaterThanZero",
            "LessThanZero",
            "Empty",
            "NotEmpty",
        };

        public static readonly List<string> PropertyPrimitivies = new List<string>
        {
            "int",
            "DateTime",
            "string",
            "decimal",
            "bool",
        };

        public static readonly List<string> PropertyIntValidations = new List<string>
        {
            "Required",
            "GreaterThanZero",
            "LessThanZero"
        };

        public static readonly List<string> PropertyDatetimeValidations = new List<string>
        {
            "Required",
            "Past",
            "Future"
        };

        public static readonly List<string> PropertyStringValidations = new List<string>
        {
            "Required",
            "Email",
            "MinLenght",
            "MaxLenght"
        };

        public static readonly List<string> PropertyDecimalValidations = new List<string>
        {
            "Required",
            "GreaterThanzero",
            "LessThanZero"
        };

        public static readonly List<string> PropertyBooleanValidations = new List<string>
        {
            "Required"
        };

        public static readonly List<string> ValidationTypes = new List<string>
        {
            "MinLenght",
            "MaxLenght",
            "Required",
            "Email",
            "Past",
            "Future",
            "GreaterThanZero",
            "LessThanZero"
        };

        public static readonly List<string> ValidationDependenciesOnValue = new List<string>
        {
            "MinLenght",
            "MaxLenght"
        };

        public static readonly List<string> ValidationDependenciesOnNumeralValue = new List<string>
        {
            "MinLenght",
            "MaxLenght"
        };
    }
}
