using Console.Models;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IQueryService
    {
        Task GenerateAsync(Project project);
    }

    public class QueryService : IQueryService
    {
        public async Task GenerateAsync(Project project)
        {
            var sb = new StringBuilder();
        }
    }
}
