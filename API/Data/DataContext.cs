//namespace should be the physical location of the file, else it will have errors
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

//DataContext class is going to be derived from an entity framework class, DbContext, import it
//it allows us to query and save instances of your entities
// Identity DB Context already has a DB set for users
//So we need to tell identity DB context about the classes that we've created
//We specify all of these because we have a join table here
public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    //When an instance of class, DataContext is called, the constructor will run with the options we provide
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //AppUser will be the entity name we provide for our dbset 
    //If we call this property users, then that is going to represent the name of the table in
    //our database when it is created
    //Now we got the dbset of app user and we got our data context class, we need to tell our applicaiton in the program class

    //For like functionality, create a table Likes
    public DbSet<UserLike> Likes { get; set; }
    //For messaging functionality, create a table Messages
    public DbSet<Message> Messages { get; set; }

    //To create 2 more tables, to make the message to be read in the live chat
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    //we need to override a method provided inside class We're deriving from the DB context
    //we can override this method to further configure
    //the model that was discovered by convention from the entity types exposed in the DB sets properties
    //the resulting model may be cached and reused
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //This uses the method inside the base class DB context and passes it this builder object
        base.OnModelCreating(builder);

        //Set up the many to many relationship between users and user roles
        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        //this is how we're going to specify what its primary key is
        //made up of the two different entities, and they are the same entity
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        //Configuring the relationships
        builder.Entity<UserLike>()
        //a user can like many other users
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            //if we were to allow our users to delete their profile from our application
            //then we'd also like to delete the corresponding likes in the database as well
            .OnDelete(DeleteBehavior.Cascade);

        //Do the same for other side of the relationship
        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

        //Set up the many to many relationships for the message functionality
        builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
