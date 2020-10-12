using FluentValidation.Validators;
using System;

namespace Console.Validators.Extensions
{
    public class FutureValidator : FluentValidation.Validators.PropertyValidator
    {
        public FutureValidator() : base("{PropertyName} is older then now")
        {
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var data = (DateTime)context.PropertyValue;

            return (data >= DateTime.Now);
        }
    }
}
