using Console.Models;
using FluentValidation;

namespace Console.Validators
{
    public class PreActionValidator : AbstractValidator<PreAction>
    {
        public PreActionValidator()
        {
            RuleFor(x => x.Set)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.Property))
                .WithMessage(x => $"PreAction's \"set\" ({x.Set}) and \"property\" ({x.Property}) must be informed together");

            RuleFor(x => x.Property)
                .NotEmpty()
                .When(x => !string.IsNullOrWhiteSpace(x.Set))
                .WithMessage(x => $"PreAction's \"set\" ({x.Set}) and \"property\" ({x.Property}) must be informed together");
        }
    }
}
