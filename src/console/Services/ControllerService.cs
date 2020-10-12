using Console.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IControllerService
    {
        Task<IList<File>> GenerateAsync(Project project);
    }

    public class ControllerService : IControllerService
    {
        public async Task<IList<File>> GenerateAsync(Project project)
        {
            return new List<File>();
        }
    }
}
