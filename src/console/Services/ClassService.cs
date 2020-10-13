using Console.Models;
using System.Collections.Generic;
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

            foreach (var property in entity.Properties)
            {
                sb.AppendLine(GenerateProperty(property));
            }

            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Domain/Models/{entity.Name}.cs"
            };
        }

        private string GenerateProperty(Property property)
        {
            var primitive = Constants.PropertyPrimitivies.Find(x => x.ToLower() == property.Primitive.ToLower());
            var nullable = property.Nullable ? "?" : string.Empty;

            if (primitive == "string")
            {
                nullable = "";
            }

            return $"        public {primitive}{nullable} {property.Name} {{ get; set; }}";
        }
    }
}
