//namespace should be the physical location of the file, else it will have errors
namespace API.Entities;

public class AppUser
{
    //public properties means other classes could access to the get and set of the properties
    //These properties represents the coloumns in our database, Id will automatically be primary key in the database
    public int Id {get; set;}

    public string UserName {get; set;}
    public byte[] PasswordHash{get; set;}
    public byte[] PasswordSalt{get; set;}
}
