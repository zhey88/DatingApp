//namespace should be the physical location of the file, else it will have errors
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    public class AppUser : IdentityUser<int>
    {
        //public properties means other classes could access to the get and set of the properties
        //These properties represents the coloumns in our database, 
        //Id will automatically be primary key in the database

        //DataOnly allows us to only track the date of something
        public DateOnly DateOfBirth {get; set;}
        //The user wants to be known as what
        public string KnownAs {get; set;}
        //The date and time of when the user is created, GMT time
        public DateTime Created {get; set;} = DateTime.UtcNow;
        //When the user is last active
        public DateTime LastActive {get; set;} = DateTime.UtcNow;
        //
        public string Gender {get; set;}
        public string Introduction {get; set;}
        public string LookingFor {get; set;}
        public string Interests {get; set;}
        public string City {get; set;}
        public string Country {get; set;}
        public List<Photo> Photos {get; set;} = new();

        //Need a join table between another app user
        //a many relationship with liked by users
        //a many relationship to like users
        public List<UserLike> LikedByUsers { get; set; }
        public List<UserLike> LikedUsers { get; set; }
        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }
        //ICollection is a type of list, roles refers to Adminstrator and etc
        public ICollection<AppUserRole> UserRoles { get; set; }

        //we need to remove the GetAge method from the AppUser.cs file to 
        //stop the database from querying the hash and salt password
        //public int GetAge()
        //{
         //  return DateOfBirth.CalculateAge();
        //}
    }

}
