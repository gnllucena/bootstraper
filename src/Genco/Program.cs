using Console.Models;
using Console.Services;
using Console.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();

        public static IHost BuildHost(string[] args) => new HostBuilder()
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IValidator<Project>, ProjectValidator>();
                services.AddSingleton<IValidator<Depends>, DependsValidator>();
                services.AddSingleton<IValidator<Entity>, EntityValidator>();
                services.AddSingleton<IValidator<Property>, PropertyValidator>();
                services.AddSingleton<IValidator<Validation>, ValidationValidator>();
                services.AddSingleton<IValidator<PreAction>, PreActionValidator>();

                services.AddTransient<IArgsService, ArgsService>();
                services.AddTransient<IOrchestratorService, OrchestratorService>();
                services.AddTransient<IClassService, ClassService>();
                services.AddTransient<IRepositoryService, RepositoryService>();
                services.AddTransient<IQueryService, QueryService>();
                services.AddTransient<IServiceService, ServiceService>();
                services.AddTransient<IControllerService, ControllerService>();
                services.AddTransient<IValidatorService, ValidatorService>();
                services.AddTransient<IFileService, FileService>();
                services.AddTransient<IValidationService, ValidationService>();
            })
            .UseSerilog()
            .Build();

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(LogEventLevel.Debug)
                .CreateLogger();

            try
            {
                var host = BuildHost(args);

                var argsService = host.Services.GetService<IArgsService>();
                var orchestratorService = host.Services.GetService<IOrchestratorService>();

#if (DEBUG)
                var configuration = new Configuration()
                {
                    File = "appsettings.json",
                    Output = "./output/"
                };
#else
                var configuration = argsService.GetArgs(args);
#endif

                await orchestratorService.OrchestrateAsync(configuration);
            }
            catch (ArgumentException ex)
            {
                Log.Logger.Information(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
