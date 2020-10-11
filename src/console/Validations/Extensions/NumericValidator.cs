using FluentValidation.Validators;

namespace Console.Validations.Extensions
{
    public class NumericValidator : PropertyValidator
    {
        public NumericValidator() : base("{PropertyName} is not a number")
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var texto = context.PropertyValue as string;

            if (!string.IsNullOrWhiteSpace(texto))
            {
                return long.TryParse(texto, out long valor);
            }

            return false;
        }
    }
}
