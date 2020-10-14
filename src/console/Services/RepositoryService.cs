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
            var primaryKey = entity.Properties.Where(x => x.PrimaryKey).First();

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
            sb.AppendLine($"namespace {project.Name}.Repositories");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public interface I{entity.Name}Repository");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        Task<int> InsertAsync({entity.Name} {entity.Name.ToLower()});");
            sb.AppendLine($"        Task UpdateAsync(int {entity.Name.ToLower()}Id, {entity.Name} {entity.Name.ToLower()});");
            sb.AppendLine($"        Task DeleteAsync(int {entity.Name}Id);");
            sb.AppendLine($"        Task<{entity.Name}> GetAsync(int {entity.Name.ToLower()}Id);");
            sb.AppendLine($"        Task<Pagination<{entity.Name}>> PaginateAsync(int offset, int limit);");
            sb.AppendLine(GenerateInterfaceMethod(entity, primaryKey));
            sb.AppendLine($"    }}");
            sb.AppendLine($"");
            sb.AppendLine($"    public class {entity.Name}Repository : I{entity.Name}Repository");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        private readonly ILogger<{entity.Name}Repository> _logger;");
            sb.AppendLine($"        private readonly ISqlService _sqlService;");
            sb.AppendLine($"");
            sb.AppendLine($"        public FileRepository(");
            sb.AppendLine($"            ILogger<FileRepository> logger,");
            sb.AppendLine($"            ISqlService sqlService");
            sb.AppendLine($"        )");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
            sb.AppendLine($"            _sqlService = sqlService ?? throw new ArgumentNullException(nameof(sqlService));");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine(GenerateInsertMethod(entity));
            sb.AppendLine($"");
            sb.AppendLine(GenerateUpdateMethod(entity));
            sb.AppendLine($"");
            sb.AppendLine(GenerateDeleteMethod(entity));
            sb.AppendLine($"");
            sb.AppendLine(GenerateGetMethod(entity));
            sb.AppendLine($"");
            sb.AppendLine(GeneratePaginationMethod(entity));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Domain/Queries/{entity.Name}Query.cs"
            };
        }

        public string GenerateInterfaceMethod(Entity entity, Property primaryKey)
        {
            var sb = new StringBuilder();

            foreach (var property in entity.Properties)
            {
                sb.AppendLine($"        Task<bool> ExistsBy{property.Name}Async(int offset, int limit);");
            }

            foreach (var property in entity.Properties)
            {
                sb.AppendLine($"        Task<bool> ExistsBy{property.Name}AndDifferentThen{primaryKey.Name}Async(int offset, int limit);");
            }

            return sb.ToString();
        }

        public string GenerateInsertMethod(Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<int> InsertAsync(File file)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogDebug($\"Inserting file.\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var parameters = new DynamicParameters();");
            sb.AppendLine($"            parameters.Add(\"FilingId\", file.FilingId, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"FormId\", file.FormId, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Name\", file.Name, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Path\", file.Path, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Created\", DateTime.Now, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"CreatedBy\", file.CreatedBy, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Updated\", DateTime.Now, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"UpdatedBy\", file.UpdatedBy, direction: ParameterDirection.Input);");
            sb.AppendLine($"");
            sb.AppendLine($"            file.Id = await _sqlService.ExecuteScalarAsync<int>(FileQuery.INSERT, CommandType.Text, parameters);");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"File inserted: {{file.Id}}.\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return file.Id;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateUpdateMethod(Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task UpdateAsync(int fileId, File file)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogDebug($\"Updating file by id: {{fileId}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var parameters = new DynamicParameters();");
            sb.AppendLine($"            parameters.Add(\"Id\", fileId);");
            sb.AppendLine($"            parameters.Add(\"FilingId\", file.FilingId, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"FormId\", file.FormId, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Name\", file.Name, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Path\", file.Path, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"Updated\", DateTime.Now, direction: ParameterDirection.Input);");
            sb.AppendLine($"            parameters.Add(\"UpdatedBy\", file.UpdatedBy, direction: ParameterDirection.Input);");
            sb.AppendLine($"");
            sb.AppendLine($"            await _sqlService.ExecuteAsync(FileQuery.UPDATE, CommandType.Text, parameters);");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"File updated\");");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateDeleteMethod(Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task DeleteAsync(int fileId)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogDebug($\"Deleting file\");");
            sb.AppendLine($"");
            sb.AppendLine($"            await _sqlService.ExecuteAsync(FileQuery.DELETE, CommandType.Text, new {{ ");
            sb.AppendLine($"                Id = fileId ");
            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"File deleted.\");");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateGetMethod(Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<File> GetAsync(int fileId)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogDebug($\"Get file by id: {{fileId}}.\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var file = await _sqlService.QueryFirstOrDefaultAsync<File>(FileQuery.GET, CommandType.Text, new {{");
            sb.AppendLine($"                Id = fileId");
            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"Got file\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return file;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GeneratePaginationMethod(Entity entity)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<Pagination<File>> PaginateAsync(int offset, int limit)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogDebug($\"List all files paginated - offset: {{offset}} - limit: {{limit}}.\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var files = await _sqlService.QueryAsync<File>(FileQuery.PAGINATE, CommandType.Text, new");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                offset,");
            sb.AppendLine($"                limit");
            sb.AppendLine($"            }});");
            sb.AppendLine($"");
            sb.AppendLine($"            var total = await _sqlService.ExecuteScalarAsync<int>(FileQuery.COUNT, CommandType.Text);");
            sb.AppendLine($"");
            sb.AppendLine($"            var pagination = new Pagination<File>(files, offset, limit, total);");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"Total files: {{pagination.Itens.Count()}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return pagination;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        public string GenerateExistsByPropertyMethod(Entity entity)
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }

        public string GenerateExistsByPropertyAndPrimaryKeyMethod(Entity entity)
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }
    }
}
