using Console.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console.Services
{
    public interface IClassService
    {
        IList<File> Generate(Project project);
    }

    public class ClassService : IClassService
    {
        public IList<File> Generate(Project project)
        {
            var files = new List<File>();

            foreach (var entity in project.Entities)
            {
                files.Add(GenerateClass(project, entity));
            }

            return files;
        }

        private File GenerateClass(Project project, Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"using System;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {project.Name}.Domain.Models");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {entity.Name}");
            sb.AppendLine($"    {{");
            sb.AppendLine(GenerateProperty(entity));
            sb.Append(GenerateToStringOverride(entity));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Domain/Models/{entity.Name}.cs"
            };
        }

        private string GenerateProperty(Entity entity)
        {
            var sb = new StringBuilder();

            foreach (var property in entity.Properties)
            {
                var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                var nullable = property.IsNullable ? "?" : string.Empty;

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_STRING)
                {
                    nullable = "";
                }

                sb.AppendLine($"        public {primitive}{nullable} {property.Name} {{ get; set; }}");
            }

            return sb.ToString();
        }

        private string GenerateToStringOverride(Entity entity)
        {
            var properties = entity.Properties.Select(x => $"{x.Name}: {{{x.Name}}}");

            var toString = string.Join(" - ", properties);

            var sb = new StringBuilder();

            sb.AppendLine($"        public override string ToString()");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            return $\"{toString}\";");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }
    }
}
