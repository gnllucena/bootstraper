using Console.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Console.Validators
{
    public class PropertyValidator : AbstractValidator<Property>
    {
        public PropertyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(x => $"Property's \"name\" must be informed for \"{x.Column}\" property");

            RuleFor(x => x.Column)
                .NotEmpty()
                .WithMessage(x => $"Property's \"column\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.Primitive)
                .NotEmpty()
                .WithMessage(x => $"Property's \"primitive\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.PrimaryKey)
                .NotNull()
                .WithMessage(x => $"Property's \"primary key\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.Nullable)
                .NotNull()
                .WithMessage(x => $"Property's \"nullable\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.Primitive)
                .Must(x => CheckExistance(Constants.PropertyPrimitivies, x))
                .WithMessage(x => $"Property's \"primitive\" has a type ({x.Primitive}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PropertyPrimitivies)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PropertyIntValidations, x.Type)))
                .When(x => x.Primitive.ToLower() == "int" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a int type ({UsedValidationType(x.Validations, "int")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PropertyIntValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PropertyDatetimeValidations, x.Type)))
                .When(x => x.Primitive.ToLower() == "datetime" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a datetime type ({UsedValidationType(x.Validations, "datetime")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PropertyDatetimeValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PropertyStringValidations, x.Type)))
                .When(x => x.Primitive.ToLower() == "string" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a string type ({UsedValidationType(x.Validations, "string")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PropertyStringValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PropertyDecimalValidations, x.Type)))
                .When(x => x.Primitive.ToLower() == "decimal" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a decimal type ({UsedValidationType(x.Validations, "decimal")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PropertyDecimalValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PropertyBooleanValidations, x.Type)))
                .When(x => x.Primitive.ToLower() == "boolean" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a boolean type ({UsedValidationType(x.Validations, "boolean")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PropertyBooleanValidations)}");
        }

        private bool CheckExistance(List<string> list, string check)
        {
            var lowered = check.ToLower();

            foreach (var item in list)
            {
                if (item.ToLower() == lowered)
                {
                    return true;
                }
            }

            return false;
        }

        private string UsedValidationType(IList<Validation> validations, string type)
        {
            var primitiveValidations = (type.ToLower()) switch
            {
                "int" => Constants.PropertyIntValidations,
                "decimal" => Constants.PropertyDecimalValidations,
                "datetime" => Constants.PropertyDatetimeValidations,
                "string" => Constants.PropertyStringValidations,
                "bool" => Constants.PropertyBooleanValidations,
                _ => throw new InvalidOperationException("Primitive not implemented"),
            };

            var lowered = type.ToLower();

            var types = validations.Where(x => !primitiveValidations.Contains(lowered)).Select(x => x.Type);

            return string.Join(", ", types);
        }
    }
}
