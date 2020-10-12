using Console.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Console.Validators
{
    public class PropertyValidator : AbstractValidator<Property>
    {
        private readonly List<string> primitivies = new List<string>
        {
            "int",
            "datetime",
            "string",
            "decimal",
            "boolean",
        };

        private readonly List<string> intValidations = new List<string>
        {
            "required",
            "greaterthanzero",
            "lessthanzero"
        };

        private readonly List<string> datetimeValidations = new List<string>
        {
            "required",
            "past",
            "future"
        };
        
        private readonly List<string> stringValidations = new List<string>
        {
            "required",
            "email",
            "minlenght",
            "maxlenght"
        };

        private readonly List<string> decimalValidations = new List<string>
        {
            "required",
            "greaterthanzero",
            "lessthanzero"
        };

        private readonly List<string> booleanValidations = new List<string>
        {
            "required"
        };

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

            RuleFor(x => x.Nullable)
                .NotNull()
                .WithMessage(x => $"Property's \"nullable\" must be informed for \"{x.Name}\" property");

            RuleFor(x => x.Primitive)
                .Must(x => primitivies.Contains(x.ToLower()))
                .WithMessage(x => $"Property's \"primitive\" has a type ({x.Primitive}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", primitivies)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => intValidations.Contains(x.Type.ToLower())))
                .When(x => x.Primitive.ToLower() == "int" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a int type ({UsedValidationType(x.Validations, "int")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", intValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => datetimeValidations.Contains(x.Type.ToLower())))
                .When(x => x.Primitive.ToLower() == "datetime" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a datetime type ({UsedValidationType(x.Validations, "datetime")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", datetimeValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => stringValidations.Contains(x.Type.ToLower())))
                .When(x => x.Primitive.ToLower() == "string" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a string type ({UsedValidationType(x.Validations, "string")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", stringValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => decimalValidations.Contains(x.Type.ToLower())))
                .When(x => x.Primitive.ToLower() == "decimal" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a decimal type ({UsedValidationType(x.Validations, "decimal")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", decimalValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => booleanValidations.Contains(x.Type.ToLower())))
                .When(x => x.Primitive.ToLower() == "boolean" && x.Validations.Any())
                .WithMessage(x => $"Property's \"validations\" has a boolean type ({UsedValidationType(x.Validations, "boolean")}) not allowed for \"{x.Name}\" property. Allowed values: {string.Join(", ", booleanValidations)}");
        }

        private string UsedValidationType(IList<Validation> validations, string type)
        {
            var primitiveValidations = new List<string>();

            switch (type.ToLower())
            {
                case "int":
                    primitiveValidations = intValidations;
                    break;
                case "decimal":
                    primitiveValidations = decimalValidations;
                    break;
                case "datetime":
                    primitiveValidations = datetimeValidations;
                    break;
                case "string":
                    primitiveValidations = stringValidations;
                    break;
                case "boolean":
                    primitiveValidations = booleanValidations;
                    break;
            }

            var types = validations.Where(x => !primitiveValidations.Contains(x.Type.ToLower())).Select(x => x.Type);

            return string.Join(", ", types);
        }
    }
}
