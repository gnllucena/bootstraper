using System.Collections.Generic;

namespace Console.Models
{
    public class Project
    {
        public Project()
        {
            Entities = new List<Entity>();
        }

        public string Name { get; set; }
        public string Database { get; set; }
        public IList<Entity> Entities { get; set; }
    }
}
