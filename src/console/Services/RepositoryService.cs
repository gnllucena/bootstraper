using Console.Models;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IRepositoryService
    {
        Task GenerateAsync(Project project);
    }

    public class RepositoryService : IRepositoryService
    {
        public async Task GenerateAsync(Project project)
        {
            var sb = new StringBuilder();
        }
    }
}
