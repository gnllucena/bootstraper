using Console.Models;
using System.Collections.Generic;
using System.Text;

namespace Console.Services
{
    public interface IRepositoryService
    {
        IList<File> Generate(Project project);
    }

    public class RepositoryService : IRepositoryService
    {
        public IList<File> Generate(Project project)
        {
            var files = new List<File>();

            foreach (var entity in project.Entities)
            {
                files.Add(GenerateRepository(project, entity));
            }

            return files;
        }

        public File GenerateRepository(Project project, Entity entity)
        {
            var sb = new StringBuilder();

            //sb.AppendLine($"using {project.Name}.Domain.Models;");
            //sb.AppendLine($"");
            //sb.AppendLine($"namespace {project.Name}.Domain.Queries");
            //sb.AppendLine($"{{");
            //sb.AppendLine($"    public class {entity.Name}Query");
            //sb.AppendLine($"    {{");
            //sb.AppendLine(GenerateInsertQuery(entity, primaryKey));
            //sb.AppendLine(GenerateUpdateQuery(entity, primaryKey));
            //sb.AppendLine(GenerateDeleteQuery(entity, primaryKey));
            //sb.AppendLine(GenerateListQuery(entity));
            //sb.AppendLine(GenerateGetQuery(primaryKey));
            //sb.AppendLine(GeneratePaginateQuery());
            //sb.AppendLine(GeneratePaginateWhereQuery(primaryKey));
            //sb.AppendLine(GeneratePaginateCountQuery(entity));

            //foreach (var property in entity.Properties)
            //{
            //    sb.AppendLine(GenerateExistsByPropertyQuery(entity, property));
            //}

            //foreach (var property in entity.Properties)
            //{
            //    sb.AppendLine(GenerateExistsByPropertyAndDifferentPrimaryKey(entity, property, primaryKey));
            //}

            //sb.AppendLine($"    }}");
            //sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Domain/Queries/{entity.Name}Query.cs"
            };
        }
    }
}
