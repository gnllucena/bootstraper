using Console.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Console.Validations
{
    public class PropertyValidation : AbstractValidator<Property>
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
            "email"
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

        public PropertyValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Property's name must be informed");

            RuleFor(x => x.Column)
                .NotEmpty()
                .WithMessage("Property's column must be informed");

            RuleFor(x => x.Primitive)
                .NotEmpty()
                .WithMessage("Property's primitive must be informed");

            RuleFor(x => x.Nullable)
                .NotEmpty()
                .WithMessage("Property's nullable must be informed");

            RuleFor(x => x.Primitive)
                .Must(x => primitivies.Contains(x.ToLower()))
                .WithMessage($"Property`s' \"primitive\" does not match allowed values: {string.Join(", ", primitivies)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => intValidations.Contains(x.Type)))
                .When(x => x.Primitive.ToLower() == "int" && x.Validations.Any())
                .WithMessage($"Property's \"validations\" does not match with int allowed values: {string.Join(", ", intValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => datetimeValidations.Contains(x.Type)))
                .When(x => x.Primitive.ToLower() == "datetime" && x.Validations.Any())
                .WithMessage($"Property's \"validations\" does not match with datetime allowed values: {string.Join(", ", datetimeValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => stringValidations.Contains(x.Type)))
                .When(x => x.Primitive.ToLower() == "string" && x.Validations.Any())
                .WithMessage($"Property's \"validations\" does not match with string allowed values: {string.Join(", ", stringValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => decimalValidations.Contains(x.Type)))
                .When(x => x.Primitive.ToLower() == "decimal" && x.Validations.Any())
                .WithMessage($"Property's \"validations\" does not match with decimal allowed values: {string.Join(", ", decimalValidations)}");

            RuleFor(x => x.Validations)
                .Must(x => x.All(x => booleanValidations.Contains(x.Type)))
                .When(x => x.Primitive.ToLower() == "boolean" && x.Validations.Any())
                .WithMessage($"Property's \"validations\" does not match with boolean allowed values: {string.Join(", ", booleanValidations)}");
        }
    }
}
