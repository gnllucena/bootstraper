using Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Console.Services
{
    public interface IValidatorService
    {
        IList<File> Generate(Project project);
    }

    public class ValidatorService : IValidatorService
    {
        public IList<File> Generate(Project project)
        {
            var files = new List<File>();

            foreach (var entity in project.Entities)
            {
                files.Add(GenerateValidator(project, entity));
            }

            return files;
        }

        public File GenerateValidator(Project project, Entity entity)
        {
            var primaryKey = entity.Properties.Where(x => x.IsPrimaryKey).First();
            
            var sb = new StringBuilder();

            sb.AppendLine($"using {project.Name}.Domain.Models;");
            sb.AppendLine($"using {project.Name}.Domain.Repositories;");
            sb.AppendLine($"using FluentValidation;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {project.Name}.Domain.Validators");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {entity.Name}Validator : AbstractValidator<{entity.Name}>");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        public {entity.Name}Validator()");
            sb.AppendLine($"        {{");
            sb.Append(GenerateRules(entity));
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine($"        protected override void EnsureInstanceNotNull(object entity)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            if (entity == null)");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                var failure = new ValidationFailure(\"{entity.Name}\", \"{entity.Name} must be informed\", null);");
            sb.AppendLine($"");
            sb.AppendLine($"                throw new ValidationException(\"Something happend when validating {entity.Name}\", new List<ValidationFailure> {{ failure }});");
            sb.AppendLine($"            }}");
            sb.AppendLine($"        }}");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Domain/Validators/{entity.Name}Validator.cs"
            };
        }

        public string GenerateRules(Entity entity)
        {
            var sb = new StringBuilder();

            for (var i = 0; i <= entity.Properties.Count() - 1; i++)
            {
                var property = entity.Properties[i];
                
                for (var z = 0; z <= property.Validations.Count() - 1; z++)
                {
                    var validation = property.Validations[z];

                    sb.AppendLine($"            RuleFor(x => x.{property.Name})");

                    var rule = validation.Type.ToLower() switch
                    {
                        Constants.VALIDATION_MINIMUMLENGTH => GenerateMinimumLengthValidation(entity, property, validation),
                        Constants.VALIDATION_MAXIMUMLENGTH => GenerateMaximumLengthValidation(entity, property, validation),
                        Constants.VALIDATION_REQUIRED => GenerateRequiredValidation(entity, property, validation),
                        Constants.VALIDATION_EMAIL => GenerateEmailValidation(entity, property, validation),
                        Constants.VALIDATION_PAST => GeneratePastValidation(entity, property, validation),
                        Constants.VALIDATION_FUTURE => GenerateFutureValidation(entity, property, validation),
                        Constants.VALIDATION_GREATERTHAN => GenerateGreaterThanValidation(entity, property, validation),
                        Constants.VALIDATION_LESSTHAN => GenerateLessThanValidation(entity, property, validation),
                        Constants.VALIDATION_GREATERTHANOREQUALTO => GenerateGreaterThanOrEqualToValidation(entity, property, validation),
                        Constants.VALIDATION_LESSTHANOREQUALTO => GenerateLessThanOrEqualToValidation(entity, property, validation),
                        Constants.VALIDATION_EMPTY => GenerateEmptyValidation(entity, property, validation),
                        _ => throw new NotImplementedException($"Validation type \"{validation.Type}\" not implemented"),
                    };

                    sb.Append(rule);
                    sb.AppendLine($"");
                }
            }

            sb = sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public string GenerateDepends(Validation validation)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(validation.Depends.When) &&
                !string.IsNullOrWhiteSpace(validation.Depends.On))
            {
                var rule = validation.Depends.When.ToLower() switch
                {
                    Constants.DEPENDS_EMPTY => $"x => string.IsNullOrEmpty(x.{validation.Depends.On})",
                    Constants.DEPENDS_NOTEMPTY => $"x => !string.IsNullOrWhiteSpace(x.{validation.Depends.On})",
                    Constants.DEPENDS_GREATERTHANZERO => $"x => x.{validation.Depends.On} > 0",
                    Constants.DEPENDS_GREATERTHANOREQUALTOZERO => $"x => x.{validation.Depends.On} >= 0",
                    Constants.DEPENDS_LESSTHANZERO => $"x => x.{validation.Depends.On} < 0",
                    Constants.DEPENDS_LESSTHANOREQUALTOZERO => $"x => x.{validation.Depends.On} <= 0",
                    Constants.DEPENDS_EQUALTOZERO => $"x => x.{validation.Depends.On} == 0",
                    Constants.DEPENDS_DEFAULT => $"x => default(x.{validation.Depends.On})",
                    _ => throw new NotImplementedException($"Depends type \"{validation.Depends.When}\" not implemented"),
                };

                sb.AppendLine($"                .When({rule})");
            }

            return sb.ToString();
        }

        public string GenerateMinimumLengthValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .MinimumLength({validation.Value})");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The length of {entity.Name}'s {property.Name} must be at least {validation.Value} characters\");");

            return sb.ToString();
        }

        public string GenerateMaximumLengthValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .MaximumLength({validation.Value})");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The length of {entity.Name}'s {property.Name} must be {validation.Value} characters or fewer\");");

            return sb.ToString();
        }

        public string GenerateRequiredValidation(Entity entity, Property property, Validation validation)
        {
            var required = "NotEmpty";

            if (property.Primitive.ToLower() == Constants.PRIMITIVE_BOOL)
            {
                required = "NotNull";
            }

            var sb = new StringBuilder();

            sb.AppendLine($"                .{required}()");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} is required\");");

            return sb.ToString();
        }

        public string GenerateEmptyValidation(Entity entity, Property property, Validation validation)
        {
            var required = "Empty";

            if (property.Primitive.ToLower() == Constants.PRIMITIVE_BOOL)
            {
                required = "Null";
            }

            var sb = new StringBuilder();

            sb.AppendLine($"                .{required}()");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must not be filled\");");

            return sb.ToString();
        }

        public string GenerateEmailValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .EmailAddress()");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} is not a valid email address\");");

            return sb.ToString();
        }

        public string GeneratePastValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .Must(x => x < DateTime.Now)");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must be a date in the past\");");

            return sb.ToString();
        }

        public string GenerateFutureValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .Must(x => x > DateTime.Now)");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must be a date in the future\");");

            return sb.ToString();
        }

        public string GenerateGreaterThanValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .GreaterThan({validation.Value})");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must be greater than {validation.Value}\");");

            return sb.ToString();
        }

        public string GenerateLessThanValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .LessThan({validation.Value})");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must be less than {validation.Value}\");");

            return sb.ToString();
        }

        public string GenerateGreaterThanOrEqualToValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .LessThan({validation.Value})");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must be greater than or equal to {validation.Value}\");");

            return sb.ToString();
        }

        public string GenerateLessThanOrEqualToValidation(Entity entity, Property property, Validation validation)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"                .LessThanOrEqualTo({validation.Value})");
            sb.Append(GenerateDepends(validation));
            sb.AppendLine($"                .WithMessage(x => $\"The {entity.Name}'s {property.Name} must be less than or equal to {validation.Value}\");");

            return sb.ToString();
        }
    }
}
