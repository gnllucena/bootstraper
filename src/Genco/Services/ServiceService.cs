using Console.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Console.Services
{
    public interface IServiceService
    {
        IList<File> Generate(Project project);
    }

    public class ServiceService : IServiceService
    {
        public IList<File> Generate(Project project)
        {
            var files = new List<File>();

            foreach (var entity in project.Entities)
            {
                files.Add(GenerateService(project, entity));
            }

            return files;
        }

        public File GenerateService(Project project, Entity entity)
        {
            var primaryKey = entity.Properties.Where(x => x.IsPrimaryKey).First();
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);

            var sb = new StringBuilder();

            sb.AppendLine($"using {project.Name}.Domain.Models;");
            sb.AppendLine($"using {project.Name}.Domain.Repositories;");
            sb.AppendLine($"using FluentValidation;");
            sb.AppendLine($"using Microsoft.Extensions.Logging;");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using System.Threading.Tasks;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace {project.Name}.Services");
            sb.AppendLine($"{{");
            sb.Append(GenerateInterface(entity, primaryKey));
            sb.AppendLine($"");
            sb.AppendLine($"    public class {entity.Name}Service : I{entity.Name}Service");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        private readonly ILogger<{entity.Name}Service> _logger;");
            sb.AppendLine($"        private readonly IAuthenticatedService _authenticatedService;");
            sb.AppendLine($"        private readonly IValidator<{entity.Name}> _{nameCamelCaseEntity}Validator;");
            sb.AppendLine($"        private readonly IValidatorService _validatorService;");
            sb.AppendLine($"        private readonly I{entity.Name}Repository _{nameCamelCaseEntity}Repository;");
            sb.AppendLine($"");
            sb.AppendLine($"        public {entity.Name}Service(");
            sb.AppendLine($"            ILogger<{entity.Name}Service> logger,");
            sb.AppendLine($"            IAuthenticatedService authenticatedService,");
            sb.AppendLine($"            IValidator<{entity.Name}> {nameCamelCaseEntity}Validator,");
            sb.AppendLine($"            IValidatorService validatorService,");
            sb.AppendLine($"            I{entity.Name}Repository {nameCamelCaseEntity}Repository");
            sb.AppendLine($"        )");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
            sb.AppendLine($"            _authenticatedService = authenticatedService ?? throw new ArgumentNullException(nameof(authenticatedService));");
            sb.AppendLine($"            _{nameCamelCaseEntity}Validator = {nameCamelCaseEntity}Validator ?? throw new ArgumentNullException(nameof({nameCamelCaseEntity}Validator));");
            sb.AppendLine($"            _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));");
            sb.AppendLine($"            _{nameCamelCaseEntity}Repository = {nameCamelCaseEntity}Repository ?? throw new ArgumentNullException(nameof({nameCamelCaseEntity}Repository));");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.Append(GenerateInsertMethod(entity, primaryKey));
            sb.AppendLine($"");
            sb.Append(GenerateUpdateMethod(entity, primaryKey));
            sb.AppendLine($"");
            sb.Append(GenerateDeleteMethod(entity, primaryKey));
            sb.AppendLine($"");
            sb.Append(GenerateGetMethod(entity, primaryKey));
            sb.AppendLine($"");
            sb.Append(GeneratePaginateMethod(entity));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");


            return new File()
            {
                Content = sb.ToString(),
                Path = $"Services/{entity.Name}Service.cs"
            };
        }

        private string GenerateInterface(Entity entity, Property primaryKey)
        {
            var parameters = Functions.GetParametersForPaginationDeclaration(entity);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);

            var sb = new StringBuilder();

            sb.AppendLine($"    public interface I{entity.Name}Service");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        Task<{entity.Name}> InsertAsync({entity.Name} {nameCamelCaseEntity});");
            sb.AppendLine($"        Task<{entity.Name}> UpdateAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey}, {entity.Name} {nameCamelCaseEntity});");
            sb.AppendLine($"        Task DeleteAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey});");
            sb.AppendLine($"        Task<{entity.Name}> GetAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey});");
            sb.AppendLine($"        Task<Pagination<{entity.Name}>> PaginateAsync(int offset, int limit, {parameters});");
            sb.AppendLine($"    }}");

            return sb.ToString();
        }

        private string GenerateInsertMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var uniqueProperties = entity.Properties.Where(x => x.IsUnique).ToList();

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<{entity.Name}> InsertAsync({entity.Name} {nameCamelCaseEntity})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is starting {{nameof({entity.Name}Service)}}.{{nameof(InsertAsync)}}\");");
            sb.AppendLine($"");

            if (entity.PreInserts != null)
            {
                foreach (var preInsert in entity.PreInserts)
                {
                    sb.AppendLine($"            {nameCamelCaseEntity}.{preInsert.Property} = {preInsert.Set};");
                }
            }
            
            sb.AppendLine($"");

            if (uniqueProperties.Any())
            {
                sb.AppendLine($"            _validatorService.Validate(_{nameCamelCaseEntity}Validator.Validate({nameCamelCaseEntity}), new List<(bool, string)>");
                sb.AppendLine($"            {{ ");

                for (var i = 0; i <= uniqueProperties.Count() - 1; i++)
                {
                    var uniqueProperty = uniqueProperties[i];
                    var comma = i == uniqueProperties.Count - 1 ? "" : ",";

                    sb.AppendLine($"                (await _{nameCamelCaseEntity}Repository.ExistsBy{uniqueProperty.Name}Async({nameCamelCaseEntity}.{uniqueProperty.Name}), $\"{uniqueProperty.Name} {{{nameCamelCaseEntity}.{uniqueProperty.Name}}} already in use\"){comma}");
                }

                sb.AppendLine($"            }});");
            }
            else
            {
                sb.AppendLine($"            _validatorService.Validate(_{nameCamelCaseEntity}Validator.Validate({nameCamelCaseEntity}));");
            }

            sb.AppendLine($"");
            sb.AppendLine($"            var {nameCamelCasePrimaryKey} = await _{nameCamelCaseEntity}Repository.InsertAsync({nameCamelCaseEntity});");
            sb.AppendLine($"");
            sb.AppendLine($"            var result = await GetAsync({nameCamelCasePrimaryKey});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"End of {{nameof({entity.Name}Service)}}.{{nameof(InsertAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GenerateUpdateMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);
            var uniqueProperties = entity.Properties.Where(x => x.IsUnique).ToList();

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<{entity.Name}> UpdateAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey}, {entity.Name} {nameCamelCaseEntity})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is starting {{nameof({entity.Name}Service)}}.{{nameof(UpdateAsync)}}\");");
            sb.AppendLine($"");

            if (entity.PreUpdates != null)
            {
                foreach (var preUpdate in entity.PreUpdates)
                {
                    sb.AppendLine($"            {nameCamelCaseEntity}.{preUpdate.Property} = {preUpdate.Set};");
                }
            }
            
            sb.AppendLine($"");

            if (uniqueProperties.Any())
            {
                sb.AppendLine($"            _validatorService.Validate(_{nameCamelCaseEntity}Validator.Validate({nameCamelCaseEntity}), new List<(bool, string)>");
                sb.AppendLine($"            {{ ");

                for (var i = 0; i <= uniqueProperties.Count() - 1; i++)
                {
                    var uniqueProperty = uniqueProperties[i];
                    
                    if (primaryKey.Name != uniqueProperty.Name)
                    {
                        var comma = i == uniqueProperties.Count - 1 ? "" : ",";

                        sb.AppendLine($"                (await _{nameCamelCaseEntity}Repository.ExistsBy{uniqueProperty.Name}AndDifferentThan{primaryKey.Name}Async({nameCamelCaseEntity}.{uniqueProperty.Name}, {nameCamelCasePrimaryKey}), $\"{uniqueProperty.Name} {{{nameCamelCaseEntity}.{uniqueProperty.Name}}} already in use\"){comma}");
                    }
                }

                sb.AppendLine($"            }});");
            }
            else
            {
                sb.AppendLine($"            _validatorService.Validate(_{nameCamelCaseEntity}Validator.Validate({nameCamelCaseEntity}));");
            }

            
            sb.AppendLine($"");
            sb.AppendLine($"            await _{nameCamelCaseEntity}Repository.UpdateAsync({nameCamelCasePrimaryKey}, {nameCamelCaseEntity});");
            sb.AppendLine($"");
            sb.AppendLine($"            var result = await GetAsync({nameCamelCasePrimaryKey});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"End of {{nameof({entity.Name}Service)}}.{{nameof(UpdateAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GenerateDeleteMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task DeleteAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is starting {{nameof({entity.Name}Service)}}.{{nameof(DeleteAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            await _{nameCamelCaseEntity}Repository.DeleteAsync({nameCamelCasePrimaryKey});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"End of {{nameof({entity.Name}Service)}}.{{nameof(DeleteAsync)}}\");");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GenerateGetMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitivePrimaryKey = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<{entity.Name}> GetAsync({primitivePrimaryKey} {nameCamelCasePrimaryKey})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is starting {{nameof({entity.Name}Service)}}.{{nameof(GetAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            var result = await _{nameCamelCaseEntity}Repository.GetAsync({nameCamelCasePrimaryKey});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"End of {{nameof({entity.Name}Service)}}.{{nameof(GetAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GeneratePaginateMethod(Entity entity)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var parameters = Functions.GetParametersForPaginationDeclaration(entity);
            var calling = Functions.GetParametersForPaginationCalling(entity);
            var properties = entity.Properties.Select(x => $"{x.Name}: {{{x.Name}}}");

            var sb = new StringBuilder();

            sb.AppendLine($"        public async Task<Pagination<{entity.Name}>> PaginateAsync(int offset, int limit, {parameters})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation($\"User {{_authenticatedService.GetUserKey()}} is starting {{nameof({entity.Name}Service)}}.{{nameof(PaginateAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            if (limit > 100)");
            sb.AppendLine($"            {{");
            sb.AppendLine($"                limit = 100;");
            sb.AppendLine($"            }}");
            sb.AppendLine($"");
            sb.AppendLine($"            var result = await _{nameCamelCaseEntity}Repository.PaginateAsync(offset, limit, {calling});");
            sb.AppendLine($"");
            sb.AppendLine($"            _logger.LogDebug($\"End of {{nameof({entity.Name}Service)}}.{{nameof(PaginateAsync)}}\");");
            sb.AppendLine($"");
            sb.AppendLine($"            return result;");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }
    }
}
