using Console.Models;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validators
{
    public class DependsValidator : AbstractValidator<Depends>
    {
        private readonly List<string> whens = new List<string>
        {
            "greaterthenzero",
            "lessthenzero",
            "empty",
            "notempty",
        };

        public DependsValidator()
        {
            RuleFor(x => x.On)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.When))
                .WithMessage(x => $"Depends' \"on\" and \"when\" must be informed together");

            RuleFor(x => x.When)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.On))
                .WithMessage(x => $"Depends' \"on\" and \"when\" must be informed together");

            RuleFor(x => x.When)
                .Must(x => whens.Contains(x.ToLower()))
                .When(x => !string.IsNullOrWhiteSpace(x.When))
                .WithMessage(x => $"Depends' \"when\" has a value not allowed. Allowed values: {string.Join(", ", whens)}");
        }
    }
}
