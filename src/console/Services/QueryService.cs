using Console.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IQueryService
    {
        Task<IList<File>> GenerateAsync(Project project);
    }

    public class QueryService : IQueryService
    {
        public async Task<IList<File>> GenerateAsync(Project project)
        {
            return new List<File>();
        }
    }
}
