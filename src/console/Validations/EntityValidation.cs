using Console.Models;
using FluentValidation;

namespace Console.Validations
{
    public class EntityValidation : AbstractValidator<Entity>
    {
        public EntityValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Entity's name must be informed");

            RuleFor(x => x.Table)
                .NotEmpty()
                .WithMessage("Entity's table must be informed");
            
            RuleFor(x => x.Properties)
                .NotEmpty()
                .WithMessage("Entity's properties must be informed");
        }
    }
}
