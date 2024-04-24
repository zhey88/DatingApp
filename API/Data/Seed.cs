using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

//Apply this seed data functions in the program.cs
namespace API.Data
{
    public class Seed
    {
        //To seed the user data into our database
        //we're injecting our data context to go ahead, check to see if we've
        //got any users and then save that stuff into our database.
        //now we've got access to something that we can use from ASP.NET Identity.
        //user manager service that we can take advantage of
        //UserManger provides the APIs for managing users in a persistent store
        //We could save the user data, delete the user, confirm user email, check password etc
    public static async Task SeedUsers(UserManager<AppUser> userManager, 
                    RoleManager<AppRole> roleManager)
            {
                //first thing we want to do is to check our database to see if we have any users inside there already,
                //because we don't want to keep seeding data into our database if we already have it
                //if we have users already, we going to stop the execution of this method
                //Check if we have any user using userManger
                if (await userManager.Users.AnyAsync()) return;

                //Read the data in the file
                var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

                //just in case we have made a mistake with casing inside our seed data
                var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

                //we want to go from Json into a C-sharp object and then we specify the type of
                //thing we want to deserialize this into, which is going to be a list of app user.
                var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

                //Create roles for users
                var roles = new List<AppRole>
                {
                    new AppRole{Name = "Member"},
                    new AppRole{Name = "Admin"},
                    new AppRole{Name = "Moderator"},
                };

                //To add roles into our database using roleManager
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                //because these are our users, we're going to need to generate passwords for them
                foreach (var user in users)
                {
                    //Change the username to lowercase to decide a username is unique or not
                    user.UserName = user.UserName.ToLower();
                    //create a user using userManager to save a user to database with a specified password
                    //Withe the userManger.createAsync, we do not need to call context.SaveChangesAsync
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    //can add our users into the roles, make the user as a member role
                    await userManager.AddToRoleAsync(user, "Member");
                }

                //Create a admin user
                var admin = new AppUser
                {
                    UserName = "admin"
                };

                await userManager.CreateAsync(admin, "Pa$$w0rd");
                //AddToRolesAsync because we want to assign admin with different roles
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
    }
}
