
using System.Collections.Generic;

namespace Model
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Level> Levels { get; set; }
        public override string ToString()
        {
            return "Id : " + Id +
                   "\n Name : " + Name;
        }
    }
}