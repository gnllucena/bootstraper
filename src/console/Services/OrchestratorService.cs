using Console.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IOrchestratorService
    {
        Task OrchestrateAsync();
    }

    public class OrchestratorService : IOrchestratorService
    {
        public async Task OrchestrateAsync()
        {
            using var stream = File.OpenRead("./apprules.json");

            var rules = await JsonSerializer.DeserializeAsync<List<Entity>>(stream);
        }
    }
}
