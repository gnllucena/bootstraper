using Console.Models;
using FluentValidation;

namespace Console.Validators
{
    public class ProjectValidator : AbstractValidator<Project>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(x => $"Project's \"name\" must be informed");

            RuleFor(x => x.Entities)
                .NotEmpty()
                .WithMessage(x => $"Project's \"entities\" must be informed for \"{x.Name}\" project");
        }
    }
}
