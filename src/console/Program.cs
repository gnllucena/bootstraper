using console.Models;
using Console.Models;
using Console.Services;
using Console.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
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

                services.AddTransient<IOrchestratorService, OrchestratorService>();
                services.AddTransient<IClassService, ClassService>();
                services.AddTransient<IRepositoryService, RepositoryService>();
                services.AddTransient<IQueryService, QueryService>();
                services.AddTransient<IServiceService, ServiceService>();
                services.AddTransient<IControllerService, ControllerService>();
                services.AddTransient<IValidatorService, ValidatorService>();
                services.AddTransient<IFileService, FileService>();
            })
            .UseSerilog()
            .Build();

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(Configuration)
               .CreateLogger();

            try
            {
                var host = BuildHost(args);

                var orchestrator = host.Services.GetService<IOrchestratorService>();

                await orchestrator.OrchestrateAsync(new Configuration() {
                    File = "./appproject.json",
                    Output = "/output/"
                });
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
