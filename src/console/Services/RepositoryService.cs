using Console.Models;
using System.Collections.Generic;
using System.Linq;
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
            var primaryKey = entity.Properties.Where(x => x.IsPrimaryKey).First();
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);

            var sb = new StringBuilder();

            sb.AppendLine($"using Dapper;");
            sb.AppendLine($"using Microsoft.Extensions.Logging;");
            sb.AppendLine($"using {project.Name}.Domain.Models;");
            sb.AppendLine($"using {project.Name}.Domain.Queries;");
            sb.AppendLine($"using {project.Name}.Services;");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Data;");
            sb.AppendLine($"using System.Linq;");
            sb.AppendLine($"using System.Threading.Tasks;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {project.Name}.Domain.Repositories");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public interface I{entity.Name}Repository");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        Task<int> InsertAsync({entity.Name} {entity.Name.ToLower()});");
            sb.AppendLine($"        Task UpdateAsync(int {nameCamelCasePrimaryKey}, {entity.Name} {entity.Name.ToLower()});");
            sb.AppendLine($"        Task DeleteAsync(int {nameCamelCasePrimaryKey});");
            sb.AppendLine($"        Task<{entity.Name}> GetAsync(int {nameCamelCasePrimaryKey});");
            sb.Append(GenerateInterfaceMethod(entity, primaryKey));
            sb.AppendLine($"    }}");
            sb.AppendLine($"");
            sb.AppendLine($"    public class {entity.Name}Repository : I{entity.Name}Repository");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        private readonly ILogger<{entity.Name}Repository> _logger;");
            sb.AppendLine($"        private readonly ISqlService _sqlService;");
            sb.AppendLine($"        private readonly IAuthenticatedService _authenticatedService;");
            sb.AppendLine($"");
            sb.AppendLine($"        public {entity.Name}Repository(");
            sb.AppendLine($"            ILogger<{entity.Name}Repository> logger,");
            sb.AppendLine($"            ISqlService sqlService,");
            sb.AppendLine($"            IAuthenticatedService authenticatedService");
            sb.AppendLine($"        )");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
            sb.AppendLine($"            _sqlService = sqlService ?? throw new ArgumentNullException(nameof(sqlService));");
            sb.AppendLine($"            _authenticatedService = authenticatedService ?? throw new ArgumentNullException(nameof(authenticatedService));");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine(GenerateInsertMethod(entity, primaryKey));
            sb.AppendLine(GenerateUpdateMethod(entity, primaryKey));
            sb.AppendLine(GenerateDeleteMethod(entity, primaryKey));
            sb.AppendLine(GenerateGetMethod(entity, primaryKey));
            sb.AppendLine(GeneratePaginationMethod(entity));
            sb.AppendLine(GenerateExistsByPropertyMethod(entity));
            sb.Append(GenerateExistsByPropertyAndDifferentPrimaryKeyMethod(entity, primaryKey));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Domain/Repositories/{entity.Name}Repository.cs"
            };
        }

        public string GenerateInterfaceMethod(Entity entity, Property primaryKey)
        {
            var sb = new StringBuilder();

            var parameters = string.Empty;

            foreach (var property in entity.Properties)
            {
                var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);
                var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                var nullable = "?";

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_STRING)
                {
                    nullable = "";
                }

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    parameters += $", DateTime? from{property.Name}, DateTime? to{property.Name}";
                }
                else
                {
                    parameters += $", {primitive}{nullable} {nameCamelCaseProperty}";
                }
            }

            sb.AppendLine($"        Task<Pagination<{entity.Name}>> PaginateAsync(int offset, int limit{parameters});");

            foreach (var property in entity.Properties)
            {
                var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                var camelCase = Functions.GetCamelCaseValue(property.Name);

                sb.AppendLine($"        Task<bool> ExistsBy{property.Name}Async({primitive} {camelCase});");
            }

            foreach (var property in entity.Properties)
            {
                if (property.Name != primaryKey.Name)
                {
                    var primitiveProperty = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                    var camelCaseProperty = Functions.GetCamelCaseValue(property.Name);

                    var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);
                    var camelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);

                    sb.AppendLine($"        Task<bool> ExistsBy{property.Name}AndDifferentThan{primaryKey.Name}Async({primitiveProperty} {camelCaseProperty}, {primitivePrimaryKey} {camelCasePrimaryKey});");
                }
            }

            return sb.ToString();
        }

        public string GenerateInsertMethod(Entity entity, Property primaryKey)
        {
            var entityCamelCase = Functions.GetCamelCaseValue(entity.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<{primaryKey.Primitive}> InsertAsync({entity.Name} {entityCamelCase})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is inserting a new {entity.Name} - {{{entityCamelCase}.ToString()}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var parameters = new DynamicParameters();");
            sb.AppendLine($"            parameters.Add(\"{primaryKey.Column}\", {entityCamelCase}.{primaryKey.Name}, direction: ParameterDirection.Output);");

            foreach (var property in entity.Properties)
            {
                if (property.Name != primaryKey.Name)
                {
                    sb.AppendLine($"            parameters.Add(\"{property.Column}\", {entityCamelCase}.{property.Name}, direction: ParameterDirection.Input);");
                }
            }
            
            sb.AppendLine($"");
            sb.AppendLine($"            var result = await _sqlService.ExecuteScalarAsync<{primitivePrimaryKey}>({entity.Name}Query.INSERT, CommandType.Text, parameters);");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"{entity.Name} {{result}} inserted\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateUpdateMethod(Entity entity, Property primaryKey)
        {
            var entityCamelCase = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task UpdateAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey}, {entity.Name} {entityCamelCase})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is updating {entity.Name} {{{nameCamelCasePrimaryKey}}} - {{{entityCamelCase}.ToString()}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            await _sqlService.ExecuteAsync({entity.Name}Query.UPDATE, CommandType.Text, new");
            sb.AppendLine($"            {{");

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];
                var comma = i == entity.Properties.Count - 1 ? "" : ",";

                if (property.Name == primaryKey.Name)
                {
                    sb.AppendLine($"                {primaryKey.Column} = {nameCamelCasePrimaryKey}{comma}");
                }
                else
                {
                    sb.AppendLine($"                {property.Column} = {entityCamelCase}.{property.Name}{comma}");
                }
            }

            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"{entity.Name} {{{nameCamelCasePrimaryKey}}} updated\");");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateDeleteMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task DeleteAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is deleting {entity.Name} {{{nameCamelCasePrimaryKey}}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            await _sqlService.ExecuteAsync({entity.Name}Query.DELETE, CommandType.Text, new");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                {primaryKey.Column} = {nameCamelCasePrimaryKey}");
            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"{entity.Name} {{{nameCamelCasePrimaryKey}}} deleted\");");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateGetMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<{entity.Name}> GetAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is getting {entity.Name} {{{nameCamelCasePrimaryKey}}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var result = await _sqlService.QueryFirstOrDefaultAsync<{entity.Name}>({entity.Name}Query.GET, CommandType.Text, new");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                {primaryKey.Column} = {nameCamelCasePrimaryKey}");
            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"Got {entity.Name} {{{nameCamelCasePrimaryKey}}} - {{result.ToString()}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GeneratePaginationMethod(Entity entity)
        {
            var parameters = string.Empty;
            var log = string.Empty;

            foreach (var property in entity.Properties)
            {
                var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);
                var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                var nullable = "?";

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_STRING)
                {
                    nullable = "";
                }

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    parameters += $", DateTime? from{property.Name}, DateTime? to{property.Name}";
                    log += $" - from{property.Name}: {{from{property.Name}}} - to{property.Name}: {{to{property.Name}}}";
                }
                else
                {
                    parameters += $", {primitive}{nullable} {nameCamelCaseProperty}";
                    log += $" - {nameCamelCaseProperty}: {{{nameCamelCaseProperty}}}";
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<Pagination<{entity.Name}>> PaginateAsync(int offset, int limit{parameters})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is paginating {entity.Name} - offset: {{offset}} - limit: {{limit}}{log}\");");
            sb.AppendLine($"");

            foreach (var property in entity.Properties)
            {
                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    sb.AppendLine($"            DateTime? parsedFrom{property.Name} = null;");
                    sb.AppendLine($"");
                    sb.AppendLine($"            if (from{property.Name} != null)");
                    sb.AppendLine($"            {{");
                    sb.AppendLine($"                parsedFrom{property.Name} = from{property.Name}.Value.Date;");
                    sb.AppendLine($"            }}");
                    sb.AppendLine($"");
                    sb.AppendLine($"            DateTime? parsedTo{property.Name} = null;");
                    sb.AppendLine($"");
                    sb.AppendLine($"            if (to{property.Name} != null)");
                    sb.AppendLine($"            {{");
                    sb.AppendLine($"                parsedTo{property.Name} = to{property.Name}.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);");
                    sb.AppendLine($"            }}");
                    sb.AppendLine($"");
                }
            }

            sb.AppendLine($"            var result = await _sqlService.QueryAsync<{entity.Name}>({entity.Name}Query.PAGINATE, CommandType.Text, new");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                offset = offset,");
            sb.AppendLine($"                limit = limit,");

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];
                var comma = i == entity.Properties.Count - 1 ? "" : ",";
                var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    sb.AppendLine($"                from{property.Name} = parsedFrom{property.Name},");
                    sb.AppendLine($"                to{property.Name} = parsedTo{property.Name}{comma}");
                }
                else
                {
                    sb.AppendLine($"                {property.Column} = {nameCamelCaseProperty}{comma}");
                }
            }

            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            var total = await _sqlService.ExecuteScalarAsync<int>({entity.Name}Query.PAGINATE_COUNT, CommandType.Text);");
            sb.AppendLine($"");
            sb.AppendLine($"            var pagination = new Pagination<{entity.Name}>(result, offset, limit, total);");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"Got pagination, the informed filter has {{pagination.Itens.Count()}} results in database\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return pagination;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateExistsByPropertyMethod(Entity entity)
        {
            var sb = new StringBuilder();

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];
                var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);
                var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);

                sb.AppendLine($"        public async Task<bool> ExistsBy{property.Name}Async({primitive} {nameCamelCaseProperty})");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is searching for a match with {{{nameCamelCaseProperty}}} in column {property.Name} on {entity.Name} table\");");
                sb.AppendLine($"");
                sb.AppendLine($"            var result = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_{property.Name.ToUpper()}, CommandType.Text, new");
                sb.AppendLine($"            {{");
                sb.AppendLine($"                {property.Column} = {nameCamelCaseProperty}");
                sb.AppendLine($"            }});");
                sb.AppendLine($"");
                sb.AppendLine($"            _logger.LogDebug(result ? \"Found a match\" : \"No match found\");");
                sb.AppendLine($"");
                sb.AppendLine($"            return result;");
                sb.AppendLine($"        }}");
                sb.AppendLine($"");
            }

            return sb.ToString();
        }

        public string GenerateExistsByPropertyAndDifferentPrimaryKeyMethod(Entity entity, Property primaryKey)
        {
            var sb = new StringBuilder();

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];

                if (property.Name != primaryKey.Name)
                {
                    var primitiveProperty = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                    var camelCaseProperty = Functions.GetCamelCaseValue(property.Name);

                    var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);
                    var camelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);

                    sb.AppendLine($"        public async Task<bool> ExistsBy{property.Name}AndDifferentThan{primaryKey.Name}Async({primitiveProperty} {camelCaseProperty}, {primitivePrimaryKey} {camelCasePrimaryKey})");
                    sb.AppendLine($"        {{");
                    sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is searching for a match with {{{camelCaseProperty}}} in column {property.Name} on {entity.Name} table with a different {primaryKey.Name} than {{{camelCasePrimaryKey}}}\");");
                    sb.AppendLine($"");
                    sb.AppendLine($"            var result = await _sqlService.ExecuteScalarAsync<bool>(UserQuery.EXISTS_BY_{property.Name.ToUpper()}, CommandType.Text, new");
                    sb.AppendLine($"            {{");
                    sb.AppendLine($"                {property.Column} = {camelCaseProperty},");
                    sb.AppendLine($"                {primaryKey.Column} = {camelCasePrimaryKey}");
                    sb.AppendLine($"            }});");
                    sb.AppendLine($"");
                    sb.AppendLine($"            _logger.LogDebug(result ? \"Found a match\" : \"No match found\");");
                    sb.AppendLine($"");
                    sb.AppendLine($"            return result;");
                    sb.AppendLine($"        }}");

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
