using Console.Models;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IServiceService
    {
        Task GenerateAsync(Project project);
    }

    public class ServiceService : IServiceService
    {
        public async Task GenerateAsync(Project project)
        {
            var sb = new StringBuilder();
        }
    }
}
