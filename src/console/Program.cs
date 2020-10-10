using Console.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using var stream = File.OpenRead("./rules.json");

            var rules = await JsonSerializer.DeserializeAsync<List<Rule>>(stream);
        }
    }
}
