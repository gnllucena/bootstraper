using Console.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IClassService
    {
        Task<IList<File>> GenerateAsync(Project project);
    }

    public class ClassService : IClassService
    {
        public async Task<IList<File>> GenerateAsync(Project project)
        {
            return new List<File>();
        }
    }
}
