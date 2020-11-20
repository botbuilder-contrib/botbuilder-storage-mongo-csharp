using System;

namespace StateManagementBot
{
    public class Marklar
    {
        public Marklar(string name = "Chip")
        {
            Name = name;
            Stamp = DateTimeOffset.Now;
        }

        public DateTimeOffset Stamp { get; set; }
        public string Name { get; set; }
    }
}
