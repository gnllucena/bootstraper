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

            RuleFor(x => x.IsPrimaryKey)
                .NotNull()
                .WithMessage(x => $"Property's \"primary key\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.IsNullable)
                .NotNull()
                .WithMessage(x => $"Property's \"nullable\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.IsUnique)
                .NotNull()
                .WithMessage(x => $"Property's \"unique\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.Primitive)
                .Must(x => CheckExistance(Constants.PROPERTY_PRIMITIVES, x))
                .WithMessage(x => $"Property's \"primitive\" has a type ({x.Primitive}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PROPERTY_PRIMITIVES)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PROPERTY_INT_VALIDATIONS, x.Type)))
                .When(x => x.Primitive.ToLower() == Constants.PRIMITIVE_INT.ToLower() && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a int type ({UsedValidationType(x.Validations, Constants.PRIMITIVE_INT)}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PROPERTY_INT_VALIDATIONS)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PROPERTY_DATETIME_VALIDATIONS, x.Type)))
                .When(x => x.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME.ToLower() && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a datetime type ({UsedValidationType(x.Validations, Constants.PRIMITIVE_DATETIME)}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PROPERTY_DATETIME_VALIDATIONS)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PROPERTY_STRING_VALIDATIONS, x.Type)))
                .When(x => x.Primitive.ToLower() == Constants.PRIMITIVE_STRING.ToLower() && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a string type ({UsedValidationType(x.Validations, Constants.PRIMITIVE_STRING)}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PROPERTY_STRING_VALIDATIONS)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PROPERTY_DECIMAL_VALIDATIONS, x.Type)))
                .When(x => x.Primitive.ToLower() == Constants.PRIMITIVE_DECIMAL.ToLower() && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a decimal type ({UsedValidationType(x.Validations, Constants.PRIMITIVE_DECIMAL)}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PROPERTY_DECIMAL_VALIDATIONS)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => CheckExistance(Constants.PROPERTY_BOOL_VALIDATIONS, x.Type)))
                .When(x => x.Primitive.ToLower() == Constants.PRIMITIVE_BOOL.ToLower() && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a boolean type ({UsedValidationType(x.Validations, Constants.PRIMITIVE_BOOL)}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", Constants.PROPERTY_BOOL_VALIDATIONS)}");
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
                Constants.PRIMITIVE_INT => Constants.PROPERTY_INT_VALIDATIONS,
                Constants.PRIMITIVE_DECIMAL => Constants.PROPERTY_DECIMAL_VALIDATIONS,
                Constants.PRIMITIVE_DATETIME => Constants.PROPERTY_DATETIME_VALIDATIONS,
                Constants.PRIMITIVE_STRING => Constants.PROPERTY_STRING_VALIDATIONS,
                Constants.PRIMITIVE_BOOL => Constants.PROPERTY_BOOL_VALIDATIONS,
                _ => throw new InvalidOperationException($"Primitive \"{type}\" not implemented"),
            };

            var types = validations.Where(x => !primitiveValidations.Contains(x.Type)).Select(x => x.Type);

            return string.Join(", ", types);
        }
    }
}
