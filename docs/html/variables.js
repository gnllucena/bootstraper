var PROJECT_DATABASE = [
  "Mysql"
];

var DEPENDS_WHEN = [
  "GreaterThanZero",
  "GreaterThanOrEqualToZero",
  "LessThanZero",
  "LessThanOrEqualToZero",
  "EqualToZero",
  "Empty",
  "NotEmpty"
];

var PROPERTY_PRIMITIVES = [
  "int",
  "DateTime",
  "string",
  "decimal",
  "bool",
];

var PROPERTY_INT_VALIDATIONS = [
  "Required",
  "Empty",
  "GreaterThan",
  "LessThan",
  "GreaterThanOrEqualTo",
  "LessThanOrEqualTo"
];

var PROPERTY_DECIMAL_VALIDATIONS = [
  "Required",
  "Empty",
  "GreaterThan",
  "LessThan",
  "GreaterThanOrEqualTo",
  "EqualToZero",
  "LessThanOrEqualTo"
];

var PROPERTY_DATETIME_VALIDATIONS = [
  "Required",
  "Empty",
  "Past",
  "Future"
];

var PROPERTY_STRING_VALIDATIONS = [
  "Required",
  "Empty",
  "Email",
  "MinimumLength",
  "MaximumLength"
];

var PROPERTY_BOOL_VALIDATIONS = [
  "Required",
  "Empty"
];

var PROPERTY_INT_DEPENDS_WHEN = [
  "GreaterThanZero",
  "GreaterThanOrEqualToZero",
  "LessThanZero",
  "LessThanOrEqualToZero",
  "EqualToZero",
  "Empty",
  "NotEmpty"
];

var PROPERTY_DECIMAL_DEPENDS_WHEN = [
  "GreaterThanZero",
  "GreaterThanOrEqualToZero",
  "LessThanZero",
  "LessThanOrEqualToZero",
  "EqualToZero",
  "Empty",
  "NotEmpty"
];

var PROPERTY_DATETIME_DEPENDS_WHEN = [
  "Empty",
  "NotEmpty"
];

var PROPERTY_STRING_DEPENDS_WHEN = [
  "Empty",
  "NotEmpty"
];

var PROPERTY_BOOL_DEPENDS_WHEN = [
  "Empty",
  "NotEmpty"
];

var VALIDATION_TYPES = [
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
];

var VALIDATION_DEPENDENCIES_ON_VALUE = [
  "MinimumLength",
  "MaximumLength",
  "GreaterThan",
  "LessThan",
  "GreaterThanOrEqualTo",
  "LessThanOrEqualTo"
];

var VALIDATION_DEPENDENCIES_ON_NUMERAL_VALUE = [
  "MinimumLength",
  "MaximumLength",
  "GreaterThan",
  "LessThan",
  "GreaterThanOrEqualTo",
  "LessThanOrEqualTo"
];
