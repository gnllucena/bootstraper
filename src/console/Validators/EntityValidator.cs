using Console.Models;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Console.Validators
{
    public class EntityValidator : AbstractValidator<Entity>
    {
        public EntityValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(x => $"Entity's \"name\" must be informed for \"{x.Table}\" entity");

            RuleFor(x => x.Table)
                .NotEmpty()
                .WithMessage(x => $"Entity's \"table\" must be informed for \"{x.Name}\" entity");
            
            RuleFor(x => x.Properties)
                .NotEmpty()
                .WithMessage(x => $"Entity's \"properties\" must be informed for \"{x.Name}\" entity");

            RuleFor(x => x.Properties)
                .Must(x => x.Count(x => x.PrimaryKey) == 1)
                .WithMessage(x => $"Entity's \"properties\" must have only one primary key for \"{x.Name}\" entity. Found: {string.Join(", ", x.Properties.Where(x => x.PrimaryKey).Select(x => x.Name))}");

            RuleFor(x => x.Properties)
                .Must(x => CheckSameExistance(x.Select(z => z.Name).ToList()))
                .WithMessage(x => $"Entity's \"properties\" have duplicated names for \"{x.Name}\" entity. Found: {SameExistanceNames(x.Properties.Select(x => x.Name).ToList())}");

            RuleFor(x => x.Properties)
                .Must(x => CheckSameExistance(x.Select(z => z.Column).ToList()))
                .WithMessage(x => $"Entity's \"properties\" have duplicated columns for \"{x.Name}\" entity. Found: {SameExistanceNames(x.Properties.Select(x => x.Column).ToList())}");
        }

        private bool CheckSameExistance(List<string> list)
        {
            for (var i = 0; i <= list.Count - 1; i++)
            {
                var current = list[i];

                for (var z = i + 1; z <= list.Count - 1; z++)
                {
                    var check = list[z];

                    if (current.ToLower() == check.ToLower())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private string SameExistanceNames(List<string> list)
        {
            var duplicates = new List<string>();

            for (var i = 0; i <= list.Count - 1; i++)
            {
                var current = list[i];

                for (var z = i + 1; z <= list.Count - 1; z++)
                {
                    var check = list[z];

                    if (current.ToLower() == check.ToLower())
                    {
                        duplicates.Add(current);

                        break;
                    }
                }
            }

            return string.Join(", ", duplicates);
        }
    }
}
