using Console.Models;
using FluentValidation;

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
        }
    }
}
