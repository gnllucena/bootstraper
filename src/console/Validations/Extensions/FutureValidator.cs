using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Console.Validations.Extensions
{
    public class FutureValidator : PropertyValidator
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
