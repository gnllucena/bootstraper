using Console.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IFileService
    {
        Task WriteAsync(Configuration configuration, IList<Models.File> files);
    }

    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(
            ILogger<FileService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task WriteAsync(Configuration configuration, IList<Models.File> files)
        {
            if (!Directory.Exists(configuration.Output))
            {
                Directory.CreateDirectory(configuration.Output);
            }

            foreach (var file in files)
            {
                CreateFileFolder(configuration.Output, file);

                var path = Path.Combine(configuration.Output, file.Path);

                _logger.LogDebug($"Creating file {path}");

                await System.IO.File.WriteAllTextAsync(path, file.Content);
            }
        }

        private void CreateFileFolder(string output, Models.File file)
        {
            var folders = file.Path.Split('/');

            var combination = output;

            foreach (var folder in folders)
            {
                if (!folder.Contains('.'))
                {
                    combination = Path.Combine(combination, folder);
                }
            }

            if (!Directory.Exists(combination))
            {
                _logger.LogDebug($"Creating folder {combination}");

                Directory.CreateDirectory(combination);
            }
        } 
    }
}
