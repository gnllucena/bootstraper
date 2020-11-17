using Console.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console.Services
{
    public interface IFileQueryService
    {
        IList<File> Generate(Project project);
    }

    public class FileQueryService : IFileQueryService
    {
        public IList<File> Generate(Project project)
        {
            var files = new List<File>();

            foreach (var entity in project.Entities)
            {
                files.Add(GenerateQuery(project, entity));
            }

            return files;
        }

        public File GenerateQuery(Project project, Entity entity)
        {
            var primaryKey = entity.Properties.Where(x => x.IsPrimaryKey).First();

            var sb = new StringBuilder();

            sb.AppendLine($"using {project.Name}.Domain.Entities;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {project.Name}.Queries");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {entity.Name}Query");
            sb.AppendLine($"    {{");
            sb.AppendLine(GenerateInsertQuery(project, entity, primaryKey));
            sb.AppendLine(GenerateUpdateQuery(entity, primaryKey));
            sb.AppendLine(GenerateDeleteQuery(entity, primaryKey));
            sb.AppendLine(GenerateListQuery(entity));
            sb.AppendLine(GenerateGetQuery(primaryKey));
            sb.AppendLine(GeneratePaginateQuery());
            sb.AppendLine(GeneratePaginateWhereQuery(primaryKey));
            sb.AppendLine(GeneratePaginateCountQuery(entity));
            sb.Append(GenerateExistsByPropertyQuery(entity));
            sb.Append(GenerateExistsByPropertyAndDifferentPrimaryKeyQuery(entity, primaryKey));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Common/Queries/{entity.Name}Query.cs"
            };
        }

        public string GenerateInsertQuery(Project project, Entity entity, Property primaryKey)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public static string INSERT = $@\"");
            sb.AppendLine($"            INSERT INTO {entity.Table}");
            sb.AppendLine($"                       (");

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];

                if (!property.IsPrimaryKey)
                {
                    var comma = i == entity.Properties.Count - 1 ? "" : ",";

                    sb.AppendLine($"                        {property.Column}{comma}");
                }
            }

            sb.AppendLine($"                       )");
            sb.AppendLine($"                VALUES (");

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];

                if (!property.IsPrimaryKey)
                {
                    var comma = i == entity.Properties.Count - 1 ? "" : ",";

                    sb.AppendLine($"                        @{property.Column}{comma}");
                }
            }

            sb.AppendLine($"                       );");
            sb.AppendLine($"");

            switch (project.Database.ToLower())
            {
                case Constants.DATABASE_MYSQL:
                    sb.AppendLine($"            SELECT LAST_INSERT_ID() as {primaryKey.Column}");
                    break;
                default:
                    throw new InvalidOperationException($"Database \"{project.Database}\" not implemented");
            }

            sb.AppendLine($"        \";");
            
            return sb.ToString();
        }

        public string GenerateUpdateQuery(Entity entity, Property primaryKey)
        {
            var maxPropertyLenght = 0;

            foreach (var property in entity.Properties)
            {
                if (property.Column.Length > maxPropertyLenght)
                {
                    maxPropertyLenght = property.Column.Length;
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine($"        public static string UPDATE = $@\"");
            sb.AppendLine($"            UPDATE {entity.Table} SET");

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];
                
                var comma = i == entity.Properties.Count - 1 ? "" : ",";

                var spaces = "";

                var moreSpaces = maxPropertyLenght - property.Column.Length;

                for (var z = 0; z <= moreSpaces - 1; z++)
                {
                    spaces += " ";
                }

                sb.AppendLine($"                   {property.Column}{spaces} = @{property.Column}{comma}");
            }

            sb.AppendLine($"             WHERE {primaryKey.Column} = @{primaryKey.Column}");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GenerateDeleteQuery(Entity entity, Property primaryKey)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public static string DELETE = $@\"");
            sb.AppendLine($"            DELETE FROM {entity.Table}");
            sb.AppendLine($"                  WHERE {primaryKey.Column} = @{primaryKey.Column}");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GenerateListQuery(Entity entity)
        {
            var maxPropertyLenght = 0;

            foreach (var property in entity.Properties)
            {
                if (property.Column.Length > maxPropertyLenght)
                {
                    maxPropertyLenght = property.Column.Length;
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine($"        public static string LIST = $@\"");
            sb.AppendLine($"            SELECT ");
            
            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];

                var comma = i == entity.Properties.Count - 1 ? "" : ",";

                var spaces = "";

                var moreSpaces = maxPropertyLenght - property.Column.Length;

                for (var z = 0; z <= moreSpaces - 1; z++)
                {
                    spaces += " ";
                }

                sb.AppendLine($"                   {property.Column}{spaces} as {{nameof({entity.Name}.{property.Name})}}{comma}");
            }

            sb.AppendLine($"              FROM {entity.Table}");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GenerateGetQuery(Property primaryKey)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public static string GET = $@\"");
            sb.AppendLine($"            {{LIST}}");
            sb.AppendLine($"             WHERE {primaryKey.Column} = @{primaryKey.Column}");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GeneratePaginateQuery()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public static string PAGINATE = $@\"");
            sb.AppendLine($"            {{LIST}}");
            sb.AppendLine($"            {{PAGINATE_WHERE}}");
            sb.AppendLine($"             LIMIT @Limit");
            sb.AppendLine($"            OFFSET @Offset");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GeneratePaginateWhereQuery(Property primaryKey)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public static string PAGINATE_WHERE = $@\"");
            sb.AppendLine($"            WHERE {primaryKey.Column} = {primaryKey.Column}");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GeneratePaginateCountQuery(Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public static string PAGINATE_COUNT = $@\"");
            sb.AppendLine($"            SELECT COUNT(1)");
            sb.AppendLine($"              FROM {entity.Table}");
            sb.AppendLine($"            {{PAGINATE_WHERE}}");
            sb.AppendLine($"        \";");

            return sb.ToString();
        }

        public string GenerateExistsByPropertyQuery(Entity entity)
        {
            var sb = new StringBuilder();

            foreach (var property in entity.Properties)
            {
                sb.AppendLine($"        public static string EXISTS_BY_{property.Name.ToUpper()} = $@\"");
                sb.AppendLine($"            SELECT COUNT(1)");
                sb.AppendLine($"              FROM {entity.Table}");
                sb.AppendLine($"             WHERE {property.Column} = @{property.Column}");
                sb.AppendLine($"        \";");
                sb.AppendLine($"");
            }
            
            return sb.ToString();
        }

        public string GenerateExistsByPropertyAndDifferentPrimaryKeyQuery(Entity entity, Property primaryKey)
        {
            var sb = new StringBuilder();

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];

                if (property.Name != primaryKey.Name)
                {
                    sb.AppendLine($"        public static string EXISTS_BY_{property.Name.ToUpper()}_AND_DIFFERENT_{primaryKey.Name.ToUpper()} = $@\"");
                    sb.AppendLine($"            SELECT COUNT(1)");
                    sb.AppendLine($"              FROM {entity.Table}");
                    sb.AppendLine($"             WHERE {property.Column} = @{property.Column}");
                    sb.AppendLine($"               AND {primaryKey.Column} <> @{primaryKey.Column}");
                    sb.AppendLine($"        \";");

                    if (i != entity.Properties.Count - 1)
                    {
                        sb.AppendLine($"");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
