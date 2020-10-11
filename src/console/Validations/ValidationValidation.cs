using Console.Models;
using Console.Validations.Extensions;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validations
{
    public class ValidationValidation : AbstractValidator<Validation>
    {
        private readonly List<string> types = new List<string>
        {
            "minlenght",
            "maxlenght",
            "required",
            "email",
            "past",
            "future",
            "greaterthanzero",
            "lessthanzero"
        };

        private readonly List<string> dependenciesOnValue = new List<string>
        {
            "minlenght",
            "maxlenght"
        };

        private readonly List<string> dependenciesOnNumeralValue = new List<string>
        {
            "minlenght",
            "maxlenght"
        };

        public ValidationValidation()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Validation's type must be informed");

            RuleFor(x => x.Type)
                .Must(x => types.Contains(x.ToLower()))
                .WithMessage($"Validation's type must be one of '{string.Join(", ", types)}'");

            RuleFor(x => x.Value)
                .NotEmpty()
                .When(x => dependenciesOnValue.Contains(x.Type.ToLower()))
                .WithMessage("Validation's value must be informed");

            RuleFor(x => x.Value)
                .ValidNumber()
                .When(x => dependenciesOnNumeralValue.Contains(x.Type.ToLower()))
                .WithMessage("Validation's value must be a number");
        }
    }
}
