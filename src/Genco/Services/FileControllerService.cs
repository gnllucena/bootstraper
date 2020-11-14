using Console.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console.Services
{
    public interface IFileControllerService
    {
        IList<File> Generate(Project project);
    }

    public class FileControllerService : IFileControllerService
    {
        public IList<File> Generate(Project project)
        {
            var files = new List<File>();

            foreach (var entity in project.Entities)
            {
                files.Add(GenerateController(project, entity));
            }

            return files;
        }

        private File GenerateController(Project project, Entity entity)
        {
            var primaryKey = entity.Properties.Where(x => x.IsPrimaryKey).First();
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);

            var sb = new StringBuilder();

            sb.AppendLine($"using {project.Name}.Domain.Entities;");
            sb.AppendLine($"using {project.Name}.Domain.Models.Responses;");
            sb.AppendLine($"using {project.Name}.Services;");
            sb.AppendLine($"using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine($"using Microsoft.AspNetCore.Mvc.ModelBinding;");
            sb.AppendLine($"using Microsoft.Extensions.Logging;");
            sb.AppendLine($"using Swashbuckle.AspNetCore.Annotations;");
            sb.AppendLine($"using System;");
            sb.AppendLine($"using System.Threading.Tasks;");
            sb.AppendLine($"");
            sb.AppendLine($"namespace API.Controllers");
            sb.AppendLine($"{{");
            sb.AppendLine($"    [ApiController]");
            sb.AppendLine($"    [Route(\"/{entity.Name.ToLower()}\")]");
            sb.AppendLine($"    [SwaggerTag(\"{entity.Name} operations\")]");
            sb.AppendLine($"    public class {entity.Name}Controller : ControllerBase");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        private readonly ILogger<{entity.Name}Controller> _logger;");
            sb.AppendLine($"        private readonly I{entity.Name}Service _{nameCamelCaseEntity}Service;");
            sb.AppendLine($"");
            sb.AppendLine($"        public {entity.Name}Controller(");
            sb.AppendLine($"            ILogger<{entity.Name}Controller> logger,");
            sb.AppendLine($"            I{entity.Name}Service {nameCamelCaseEntity}Service)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger = logger ?? throw new ArgumentNullException(nameof(logger));");
            sb.AppendLine($"            _{nameCamelCaseEntity}Service = {nameCamelCaseEntity}Service ?? throw new ArgumentNullException(nameof({nameCamelCaseEntity}Service));");
            sb.AppendLine($"        }}");
            sb.AppendLine($"");
            sb.AppendLine(GeneratePostMethod(entity));
            sb.AppendLine(GeneratePutMethod(entity, primaryKey));
            sb.AppendLine(GenerateDeleteMethod(entity, primaryKey));
            sb.AppendLine(GenerateGetMethod(entity, primaryKey));
            sb.Append(GeneratePaginateMethod(entity));
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");

            return new File()
            {
                Content = sb.ToString(),
                Path = $"Controllers/{entity.Name}Controller.cs"
            };
        }

        private string GeneratePostMethod(Entity entity)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);

            var sb = new StringBuilder();

            sb.AppendLine($"        [HttpPost]");
            sb.AppendLine($"        [SwaggerOperation(");
            sb.AppendLine($"           Summary = \"Create a new {entity.Name}\",");
            sb.AppendLine($"           Description = \"Create a new {entity.Name}\"");
            sb.AppendLine($"        )]");
            sb.AppendLine($"        [SwaggerResponse(201, \"{entity.Name} created\", typeof({entity.Name}))]");
            sb.AppendLine($"        public async Task<ActionResult> Post(");
            sb.AppendLine($"            [SwaggerParameter(\"The new {entity.Name}\")][FromBody] {entity.Name} {nameCamelCaseEntity})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var result = await _{nameCamelCaseEntity}Service.InsertAsync({nameCamelCaseEntity});");
            sb.AppendLine($"");
            sb.AppendLine($"            return Created(new Uri($\"{{Request.Path}}/{{result.Id}}\", UriKind.Relative), result);");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GeneratePutMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        [HttpPut(\"{{{nameCamelCasePrimaryKey}}}\")]");
            sb.AppendLine($"        [SwaggerOperation");
            sb.AppendLine($"        (");
            sb.AppendLine($"          Summary = \"Update {entity.Name} information\",");
            sb.AppendLine($"          Description = \"Update {entity.Name} information\"");
            sb.AppendLine($"        )]");
            sb.AppendLine($"        [SwaggerResponse(200, \"{entity.Name} updated\", typeof({entity.Name}))]");
            sb.AppendLine($"        public async Task<ActionResult> Put(");
            sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name} {primaryKey.Name}\")][BindRequired] {primitive} {nameCamelCasePrimaryKey},");
            sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name} to be updated\")][FromBody] {entity.Name} {nameCamelCaseEntity})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var result = await _{nameCamelCaseEntity}Service.UpdateAsync({nameCamelCasePrimaryKey}, {nameCamelCaseEntity});");
            sb.AppendLine($"");
            sb.AppendLine($"            return Ok(result);");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GenerateDeleteMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        [HttpDelete(\"{{{nameCamelCasePrimaryKey}}}\")]");
            sb.AppendLine($"        [SwaggerOperation");
            sb.AppendLine($"        (");
            sb.AppendLine($"           Summary = \"Delete {entity.Name}\",");
            sb.AppendLine($"           Description = \"Delete {entity.Name}\"");
            sb.AppendLine($"        )]");
            sb.AppendLine($"        [SwaggerResponse(204, \"{entity.Name} deleted\")]");
            sb.AppendLine($"        public async Task<ActionResult> Delete(");
            sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name} {primaryKey.Name}\")][BindRequired] {primitive} {nameCamelCasePrimaryKey})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            await _{nameCamelCaseEntity}Service.DeleteAsync({nameCamelCasePrimaryKey});");
            sb.AppendLine($"");
            sb.AppendLine($"            return NoContent();");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GenerateGetMethod(Entity entity, Property primaryKey)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var nameCamelCasePrimaryKey = Functions.GetCamelCaseValue(primaryKey.Name);
            var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, primaryKey.Primitive);

            var sb = new StringBuilder();

            sb.AppendLine($"        [HttpGet(\"{{{nameCamelCasePrimaryKey}}}\")]");
            sb.AppendLine($"        [SwaggerOperation");
            sb.AppendLine($"        (");
            sb.AppendLine($"           Summary = \"Get a single {entity.Name}\",");
            sb.AppendLine($"           Description = \"Get a single {entity.Name}\"");
            sb.AppendLine($"        )]");
            sb.AppendLine($"        [SwaggerResponse(200, \"Got {entity.Name}\", typeof({entity.Name}))]");
            sb.AppendLine($"        public async Task<ActionResult> Get(");
            sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name} {primaryKey.Name}\")][BindRequired] {primitive} {nameCamelCasePrimaryKey})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var result = await _{nameCamelCaseEntity}Service.GetAsync({nameCamelCasePrimaryKey});");
            sb.AppendLine($"");
            sb.AppendLine($"            return Ok(result);");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }

        private string GeneratePaginateMethod(Entity entity)
        {
            var nameCamelCaseEntity = Functions.GetCamelCaseValue(entity.Name);
            var parameters = Functions.GetParametersForPaginationCalling(entity);

            var sb = new StringBuilder();

            sb.AppendLine($"        [HttpGet(\"/{entity.Name.ToLower()}\")]");
            sb.AppendLine($"        [SwaggerOperation(");
            sb.AppendLine($"            Summary = \"Get a paginated list of {entity.Name}\",");
            sb.AppendLine($"            Description = \"Get a paginated list of {entity.Name}\"");
            sb.AppendLine($"        )]");
            sb.AppendLine($"        [SwaggerResponse(200, \"A paginated list of {entity.Name}\", typeof(Pagination<{entity.Name}>))]");
            sb.AppendLine($"        public async Task<ActionResult> List(");
            sb.AppendLine($"            [SwaggerParameter(\"The offset number of {entity.Name}\")][BindRequired] int offset,");
            sb.AppendLine($"            [SwaggerParameter(\"The limit of {entity.Name} on response\")][BindRequired] int limit,");

            for (var i = 0; i <= entity.Properties.Count - 1; i++)
            {
                var property = entity.Properties[i];

                var comma = i == entity.Properties.Count - 1 ? "" : ",";
                var parentheses = i == entity.Properties.Count - 1 ? ")" : "";

                if (property.Primitive.ToLower() == Constants.PRIMITIVE_DATETIME)
                {
                    sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name}'s {property.Name} start date\")] DateTime? from{property.Name}{comma}");
                    sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name}'s {property.Name} end date\")] DateTime? to{property.Name}{comma}{parentheses}");
                }
                else
                {
                    var primitive = Functions.GetConstantValue(Constants.PROPERTY_PRIMITIVES, property.Primitive);
                    var nameCamelCaseProperty = Functions.GetCamelCaseValue(property.Name);
                    var nullable = "?";

                    if (property.Primitive.ToLower() == Constants.PRIMITIVE_STRING)
                    {
                        nullable = "";
                    }

                    sb.AppendLine($"            [SwaggerParameter(\"The {entity.Name}'s {property.Name}\")] {primitive}{nullable} {nameCamelCaseProperty}{comma}{parentheses}");
                }
            }

            sb.AppendLine($"        {{");
            sb.AppendLine($"            var result = await _{nameCamelCaseEntity}Service.PaginateAsync(offset, limit, {parameters});");
            sb.AppendLine($"");
            sb.AppendLine($"            return Ok(result);");
            sb.AppendLine($"        }}");

            return sb.ToString();
        }
    }
}
