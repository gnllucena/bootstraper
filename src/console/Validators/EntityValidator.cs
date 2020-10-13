using Console.Models;
using FluentValidation;
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
        }
    }
}
