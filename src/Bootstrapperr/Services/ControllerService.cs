using Console.Models;
using System.Collections.Generic;

namespace Console.Services
{
    public interface IControllerService
    {
        IList<File> Generate(Project project);
    }

    public class ControllerService : IControllerService
    {
        public IList<File> Generate(Project project)
        {
            return new List<File>();
        }
    }
}
