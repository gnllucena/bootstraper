using System.Collections.Generic;

namespace Console.Models
{
    public class Entity
    {
        public string Name { get; set; }
        public string Table { get; set; }
        public IList<PreAction> PreInserts { get; set; }
        public IList<PreAction> PreUpdates { get; set; }
        public IList<Property> Properties { get; set; }
    }
}
