using Console.Models;
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

        public static string GetParametersForPaginationDeclaration(Entity entity)
        {
            var parameters = string.Empty;

            foreach (var property in entity.Properties)
            {
                var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);
                var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                var nullable = "?";

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_STRING)
                {
                    nullable = "";
                }

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    parameters += $"DateTime? from{property.Name}, DateTime? to{property.Name}, ";
                }
                else
                {
                    parameters += $"{primitive}{nullable} {nameCamelCaseProperty}, ";
                }
            }

            return parameters.Substring(0, parameters.Length - 2);
        }

        public static string GetParametersForPaginationCalling(Entity entity)
        {
            var parameters = string.Empty;

            foreach (var property in entity.Properties)
            {
                var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    parameters += $"from{property.Name}, to{property.Name}, ";
                }
                else
                {
                    parameters += $"{nameCamelCaseProperty}, ";
                }
            }

            return parameters.Substring(0, parameters.Length - 2);
        }
    }
}
