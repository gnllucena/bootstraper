namespace Console.Models
{
    public class Validation
    {
        public Validation()
        {
            Depends = new Depends();
        }

        public string Type { get; set; }
        public string Value { get; set; }
        public Depends Depends { get; set; }
    }
}
