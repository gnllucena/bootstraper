using System.Collections.Generic;

namespace Console.Models
{
    public class Property
    {
        public Property()
        {
            Validations = new List<Validation>();
        }

        public string Name { get; set; }
        public string Column { get; set; }
        public string Primitive { get; set; }
        public bool Nullable { get; set; }
        public IList<Validation> Validations { get; set; }
    }
}
