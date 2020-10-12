using Console.Models;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IControllerService
    {
        Task GenerateAsync(Project project);
    }

    public class ControllerService : IControllerService
    {
        public async Task GenerateAsync(Project project)
        {
            var sb = new StringBuilder();
        }
    }
}
