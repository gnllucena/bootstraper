using Console.Models;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validators
{
    public class DependsValidator : AbstractValidator<Depends>
    {
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
                .Must(x => CheckExistance(Constants.DEPENDS_WHEN, x))
                .When(x => !string.IsNullOrWhiteSpace(x.When))
                .WithMessage(x => $"Depends' \"when\" has a type ({x.When}) not allowed. Allowed values: {string.Join(", ", Constants.DEPENDS_WHEN)}");   
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
    }
}
