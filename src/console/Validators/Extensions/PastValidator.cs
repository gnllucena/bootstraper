using FluentValidation.Validators;
using System;

namespace Console.Validators.Extensions
{
    public class PastValidator : FluentValidation.Validators.PropertyValidator
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
