using Console.Models;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validations
{
    public class DependsValidation : AbstractValidator<Depends>
    {
        private readonly List<string> whens = new List<string>
        {
            "greaterthenzero",
            "lessthenzero",
            "empty",
            "notempty",
        };

        public DependsValidation()
        {
            RuleFor(x => x.On)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.When))
                .WithMessage("Depends' on and when must be informed together");

            RuleFor(x => x.When)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.On))
                .WithMessage("Depends' on and when must be informed together");

            RuleFor(x => x.When)
                .Must(x => whens.Contains(x.ToLower()))
                .When(x => !string.IsNullOrWhiteSpace(x.When))
                .WithMessage($"Depends' \"when\" does not match allowed values: {string.Join(", ", whens)}");
        }
    }
}
