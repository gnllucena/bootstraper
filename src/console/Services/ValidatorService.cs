using Console.Models;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IValidatorService
    {
        Task GenerateAsync(Project project);
    }

    public class ValidatorService : IValidatorService
    {
        public async Task GenerateAsync(Project project)
        {
            var sb = new StringBuilder();
        }
    }
}
