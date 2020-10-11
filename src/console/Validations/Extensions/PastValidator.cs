using FluentValidation.Validators;
using System;

namespace Console.Validations.Extensions
{
    public class PastValidator : PropertyValidator
    {
        public PastValidator() : base("{PropertyName} is not older then now")
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var data = (DateTime)context.PropertyValue;

            return (data <= DateTime.Now);
        }
    }
}
