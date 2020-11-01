using FluentValidation;

namespace Console.Validators.Extensions
{
    public static class Extensions
    {
        public static IRuleBuilderOptions<T, TElement> ValidNumber<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new NumericValidator());
        }

        public static IRuleBuilderOptions<T, TElement> ValidFutureDate<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new FutureValidator());
        }

        public static IRuleBuilderOptions<T, TElement> ValidPastDate<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PastValidator());
        }
    }
}
