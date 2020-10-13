using Console.Models;
using FluentValidation;
using System.Collections.Generic;

namespace Console.Validators
{
    public class ProjectValidator : AbstractValidator<Project>
    {
        public ProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(x => $"Project's \"name\" must be informed");

            RuleFor(x => x.Database)
                .NotEmpty()
                .WithMessage(x => $"Project's \"database\" must be informed for \"{x.Name}\" project");

            RuleFor(x => x.Entities)
                .NotEmpty()
                .WithMessage(x => $"Project's \"entities\" must be informed for \"{x.Name}\" project");

            RuleFor(x => x.Database)
                .Must(x => CheckExistance(Constants.ProjectDatabases, x))
                .WithMessage(x => $"Project's \"database\" has a type ({x.Database}) not allowed for \"{x.Name}\" project. Allowed values: {string.Join(", ", Constants.ProjectDatabases)}");
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
