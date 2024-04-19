using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

//Apply this seed data functions in the program.cs
namespace API.Data
{
    public class Seed
    {
        //To seed the user data into our database
    public static async Task SeedUsers(DataContext context)
            {
                //first thing we want to do is to check our database to see if we have any users inside there already,
                //because we don't want to keep seeding data into our database if we already have it
                //if we have users already, we going to stop the execution of this method
                if (await context.Users.AnyAsync()) return;

                //Read the data in the file
                var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

                //just in case we have made a mistake with casing inside our seed data
                var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};

                //we want to go from Json into a C-sharp object and then we specify the type of
                //thing we want to deserialize this into, which is going to be a list of app user.
                var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

                //because these are our users, we're going to need to generate passwords for them
                foreach (var user in users)
                {
                    using var hmac = new HMACSHA512();

                    user.UserName = user.UserName.ToLower();
                    //Make it complex passwords
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    user.PasswordSalt = hmac.Key;

                    context.Users.Add(user);
                }
                //Save the users into our database
                await context.SaveChangesAsync();
            }
    }
}
