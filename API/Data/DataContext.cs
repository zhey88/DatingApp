//namespace should be the physical location of the file, else it will have errors
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

//DataContext class is going to be derived from an entity framework class, DbContext, import it
//it allows us to query and save instances of your entities
public class DataContext : DbContext
{
    //When an instance of class, DataContext is called, the constructor will run with the options we provide
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //AppUser will be the entity name w eprovide for our dbset 
    //If we call this property users, then that is going to represent the name of the table in
    //our database when it is created
    //Now we got the dbset of app user and we got our data context class, we need to tell our applicaiton in the program class
    public DbSet<AppUser> Users { get; set; }

    internal string CreateToken(AppUser user)
    {
        throw new NotImplementedException();
    }
}
