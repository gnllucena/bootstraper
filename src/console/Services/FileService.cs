using Console.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Console.Services
{
    public interface IFileService
    {
        Task WriteAsync(IList<File> files);
    }

    public class FileService : IFileService
    {
        public async Task WriteAsync(IList<File> files)
        {
            
        }
    }
}
