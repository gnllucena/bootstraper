using System.Collections.Generic;

namespace Console
{
    public static class Constants
    {
        public const string PRIMITIVE_INT = "int";
        public const string PRIMITIVE_DATETIME = "datetime";
        public const string PRIMITIVE_STRING = "string";
        public const string PRIMITIVE_DECIMAL = "decimal";
        public const string PRIMITIVE_BOOL = "bool";

        public const string VALIDATION_MINIMUMLENGTH = "minimumlength";
        public const string VALIDATION_MAXIMUMLENGTH = "maximumlength";
        public const string VALIDATION_REQUIRED = "required";
        public const string VALIDATION_EMAIL = "email";
        public const string VALIDATION_PAST = "past";
        public const string VALIDATION_FUTURE = "future";
        public const string VALIDATION_GREATERTHAN = "greaterthan";
        public const string VALIDATION_LESSTHAN = "lessthan";
        public const string VALIDATION_GREATERTHANOREQUALTO = "greaterthanorequalto";
        public const string VALIDATION_LESSTHANOREQUALTO = "lessthanorequalto";
        public const string VALIDATION_EMPTY = "empty";

        public const string DEPENDS_GREATERTHANZERO = "greaterthanzero";
        public const string DEPENDS_GREATERTHANOREQUALTOZERO = "greaterthanorequalto";
        public const string DEPENDS_LESSTHANZERO = "lessthanzero";
        public const string DEPENDS_LESSTHANOREQUALTOZERO = "lessthanorequalto";
        public const string DEPENDS_EMPTY = "empty";
        public const string DEPENDS_NOTEMPTY = "notempty";
        public const string DEPENDS_EQUALTOZERO = "equaltozero";

        public static readonly List<string> PROJECT_TYPE = new List<string>
        {
            "API"
        };

        public static readonly List<string> PROJECT_DATABASE = new List<string>
        {
            "Mysql"
        };

        public static readonly List<string> DEPENDS_WHEN = new List<string>
        {
            "GreaterThanZero",
            "GreaterThanOrEqualToZero",
            "LessThanZero",
            "LessThanOrEqualToZero",
            "EqualToZero",
            "Empty",
            "NotEmpty"
        };

        public static readonly List<string> PROPERTY_PRIMITIVES = new List<string>
        {
            "int",
            "DateTime",
            "string",
            "decimal",
            "bool",
        };

        public static readonly List<string> PROPERTY_INT_VALIDATIONS = new List<string>
        {
            "Required",
            "Empty",
            "GreaterThan",
            "LessThan",
            "GreaterThanOrEqualTo",
            "LessThanOrEqualTo"
        };

        public static readonly List<string> PROPERTY_DECIMAL_VALIDATIONS = new List<string>
        {
            "Required",
            "Empty",
            "GreaterThan",
            "LessThan",
            "GreaterThanOrEqualTo",
            "EqualToZero",
            "LessThanOrEqualTo"
        };

        public static readonly List<string> PROPERTY_DATETIME_VALIDATIONS = new List<string>
        {
            "Required",
            "Empty",
            "Past",
            "Future"
        };

        public static readonly List<string> PROPERTY_STRING_VALIDATIONS = new List<string>
        {
            "Required",
            "Empty",
            "Email",
            "MinimumLength",
            "MaximumLength"
        };

        public static readonly List<string> PROPERTY_BOOL_VALIDATIONS = new List<string>
        {
            "Required",
            "Empty"
        };

        public static readonly List<string> PROPERTY_INT_DEPENDS_WHEN = new List<string>
        {
            "GreaterThanZero",
            "GreaterThanOrEqualToZero",
            "LessThanZero",
            "LessThanOrEqualToZero",
            "EqualToZero",
            "Empty",
            "NotEmpty"
        };

        public static readonly List<string> PROPERTY_DECIMAL_DEPENDS_WHEN = new List<string>
        {
            "GreaterThanZero",
            "GreaterThanOrEqualToZero",
            "LessThanZero",
            "LessThanOrEqualToZero",
            "EqualToZero",
            "Empty",
            "NotEmpty"
        };

        public static readonly List<string> PROPERTY_DATETIME_DEPENDS_WHEN = new List<string>
        {
            "Empty",
            "NotEmpty"
        };

        public static readonly List<string> PROPERTY_STRING_DEPENDS_WHEN = new List<string>
        {
            "Empty",
            "NotEmpty"
        };

        public static readonly List<string> PROPERTY_BOOL_DEPENDS_WHEN = new List<string>
        {
            "Empty",
            "NotEmpty"
        };

        public static readonly List<string> VALIDATION_TYPES = new List<string>
        {
            "MinimumLength",
            "MaximumLength",
            "Required",
            "Email",
            "Past",
            "Future",
            "GreaterThan",
            "LessThan",
            "GreaterThanOrEqualTo",
            "LessThanOrEqualTo",
            "Empty"
        };

        public static readonly List<string> VALIDATION_DEPENDENCIES_ON_VALUE = new List<string>
        {
            "MinimumLength",
            "MaximumLength",
            "GreaterThan",
            "LessThan",
            "GreaterThanOrEqualTo",
            "LessThanOrEqualTo"
        };

        public static readonly List<string> VALIDATION_DEPENDENCIES_ON_NUMERAL_VALUE = new List<string>
        {
            "MinimumLength",
            "MaximumLength",
            "GreaterThan",
            "LessThan",
            "GreaterThanOrEqualTo",
            "LessThanOrEqualTo"
        };
    }
}
