using console.Models;
using Console.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IOrchestratorService
    {
        Task OrchestrateAsync(Configuration configuration);
    }

    public class OrchestratorService : IOrchestratorService
    {
        private readonly ILogger<OrchestratorService> _logger;
        private readonly IValidator<Project> _projectValidator;
        private readonly IValidator<Entity> _entityValidator;
        private readonly IValidator<Depends> _dependsValidator;
        private readonly IValidator<Property> _propertyValidator;
        private readonly IValidator<Validation> _validationValidator;
        private readonly IClassService _classService;
        private readonly IQueryService _queryService;
        private readonly IRepositoryService _repositoryService;
        private readonly IServiceService _serviceService;
        private readonly IValidatorService _validatorService;
        private readonly IControllerService _controllerService;
        private readonly IFileService _fileService;

        public OrchestratorService(
            ILogger<OrchestratorService> logger,
            IValidator<Entity> entityValidator,
            IValidator<Project> projectValidator,
            IValidator<Depends> dependsValidator,
            IValidator<Property> propertyValidator,
            IValidator<Validation> validationValidator,
            IClassService classService,
            IQueryService queryService,
            IRepositoryService repositoryService,
            IServiceService serviceService,
            IValidatorService validatorService,
            IControllerService controllerService,
            IFileService fileService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectValidator = projectValidator ?? throw new ArgumentNullException(nameof(projectValidator));
            _entityValidator = entityValidator ?? throw new ArgumentNullException(nameof(entityValidator));
            _dependsValidator = dependsValidator ?? throw new ArgumentNullException(nameof(dependsValidator));
            _propertyValidator = propertyValidator ?? throw new ArgumentNullException(nameof(propertyValidator));
            _validationValidator = validationValidator ?? throw new ArgumentNullException(nameof(validationValidator));
            _classService = classService ?? throw new ArgumentNullException(nameof(classService));
            _queryService = queryService ?? throw new ArgumentNullException(nameof(queryService));
            _repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));
            _serviceService = serviceService ?? throw new ArgumentNullException(nameof(serviceService));
            _validatorService = validatorService ?? throw new ArgumentNullException(nameof(validatorService));
            _controllerService = controllerService ?? throw new ArgumentNullException(nameof(controllerService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public async Task OrchestrateAsync(Configuration configuration)
        {
            _logger.LogDebug("Beginning of process");

            var project = new Project();

            try
            {
                using var stream = System.IO.File.OpenRead(configuration.File);

                project = await JsonSerializer.DeserializeAsync<Project>(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return;
            }
            
            if (HasErrorsOnJson(project))
            {
                return;
            }

            var files = new List<Models.File>();

            // --------- Domain
            // --------- |
            // --------- |- Models
            // --------- |- Queries
            // --------- |- Repositories
            // --------- |- Validators
            // --------- Services

            try
            {
                files.AddRange(_classService.Generate(project));
                files.AddRange(_queryService.Generate(project));
                files.AddRange(_repositoryService.Generate(project));
                files.AddRange(await _serviceService.GenerateAsync(project));
                files.AddRange(await _validatorService.GenerateAsync(project));
                files.AddRange(await _controllerService.GenerateAsync(project));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                return;
            }

            await _fileService.WriteAsync(files);

            _logger.LogDebug("End of process");
        }

        private bool HasErrorsOnJson(Project project)
        {
            var validations = new List<bool>();

            _logger.LogDebug($"Project \"{project.Name}\" validation");

            validations.Add(Log(_projectValidator.Validate(project)));

            foreach (var entity in project.Entities)
            {
                _logger.LogDebug($"Entity \"{entity.Name}\" from \"{project.Name}\" project validation");

                validations.Add(Log(_entityValidator.Validate(entity)));

                foreach (var property in entity.Properties)
                {
                    _logger.LogDebug($"Property \"{property.Name}\" from \"{entity.Name}\" entity validation");

                    validations.Add(Log(_propertyValidator.Validate(property)));

                    foreach (var validation in property.Validations)
                    {
                        _logger.LogDebug($"Validation \"{validation.Type}\" from \"{property.Name}\" property validation");

                        validations.Add(Log(_validationValidator.Validate(validation)));

                        _logger.LogDebug($"Dependation \"{validation.Depends.On}\" and \"{validation.Depends.When}\" from \"{validation.Type}\" validation validation");

                        validations.Add(Log(_dependsValidator.Validate(validation.Depends)));
                    }
                }
            }

            return validations.Any(x => x == true);
        }

        private bool Log(ValidationResult result)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError(error.ErrorMessage);
            }

            return result.Errors.Any();
        }
    }
}
