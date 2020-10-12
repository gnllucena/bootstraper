using Console.Models;
using Console.Validators.Extensions;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validators
{
    public class ValidationValidator : AbstractValidator<Validation>
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

        public ValidationValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage(x => $"Validation's \"type\" must be informed for \"{x.Value}\" validation");

            RuleFor(x => x.Type)
                .Must(x => types.Contains(x.ToLower()))
                .WithMessage(x => $"Validation's \"type\" not allowed for \"{x.Value}\" validation. Allowed values: {string.Join(", ", types)}");

            RuleFor(x => x.Value)
                .NotEmpty()
                .When(x => dependenciesOnValue.Contains(x.Type.ToLower()))
                .WithMessage(x => $"Validation's \"value\" must be informed for \"{x.Type}\" validation");

            RuleFor(x => x.Value)
                .ValidNumber()
                .When(x => dependenciesOnNumeralValue.Contains(x.Type.ToLower()))
                .WithMessage(x => $"Validation's \"value\" must be a number for \"{x.Type}\" validation");
        }
    }
}
