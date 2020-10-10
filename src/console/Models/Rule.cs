using System.Collections.Generic;

namespace Console.Models
{
    public class Rule
    {
        public string Entity { get; set; }
        public string Table { get; set; }
        public IList<Property> Properties { get; set; }
    }
}
