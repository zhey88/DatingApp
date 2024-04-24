using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group()
        {

        }
        public Group(string name)
        {
            Name = name;
        }

        //the name of the group should always be unique, set the group name to be primary key
        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}