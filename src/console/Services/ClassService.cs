using Console.Models;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IClassService
    {
        Task GenerateAsync(Project project);
    }

    public class ClassService : IClassService
    {
        public async Task GenerateAsync(Project project)
        {
            var sb = new StringBuilder();
        }
    }
}
