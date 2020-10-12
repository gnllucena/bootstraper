using Console.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IValidatorService
    {
        Task<IList<File>> GenerateAsync(Project project);
    }

    public class ValidatorService : IValidatorService
    {
        public async Task<IList<File>> GenerateAsync(Project project)
        {
            return new List<File>();
        }
    }
}
