using Console.Models;
using System.Collections.Generic;

namespace Console.Services
{
    public interface IServiceService
    {
        IList<File> Generate(Project project);
    }

    public class ServiceService : IServiceService
    {
        public IList<File> Generate(Project project)
        {
            return new List<File>();
        }
    }
}
