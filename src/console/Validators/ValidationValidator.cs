using Console.Models;
using Console.Validators.Extensions;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validators
{
    public class ValidationValidator : AbstractValidator<Validation>
    {
        public ValidationValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage(x => $"Validation's \"type\" must be informed for \"{x.Value}\" validation");

            RuleFor(x => x.Type)
                .Must(x => CheckExistance(Constants.ValidationTypes, x))
                .WithMessage(x => $"Validation's \"type\" not allowed for \"{x.Value}\" validation. Allowed values: {string.Join(", ", Constants.ValidationTypes)}");

            RuleFor(x => x.Value)
                .NotEmpty()
                .When(x => CheckExistance(Constants.ValidationDependenciesOnValue, x.Type))
                .WithMessage(x => $"Validation's \"value\" must be informed for \"{x.Type}\" validation");

            RuleFor(x => x.Value)
                .ValidNumber()
                .When(x => CheckExistance(Constants.ValidationDependenciesOnNumeralValue, x.Type))
                .WithMessage(x => $"Validation's \"value\" must be a number for \"{x.Type}\" validation");
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
