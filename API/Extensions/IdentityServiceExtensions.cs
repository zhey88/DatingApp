using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<AppUser>(opt =>
        {
            //we can specify the complexity, RequireNonAlphanumeric
            //means we need something that's not a number or a letter and require an uppercase character
            //We turned off that
            opt.Password.RequireNonAlphanumeric = false;
        })
            .AddRoles<AppRole>() //we need to specify role manager first
            .AddRoleManager<RoleManager<AppRole>>()
            //to create all of the tables related to identity in our database
            .AddEntityFrameworkStores<DataContext>(); //pass list to data context
            
        //So that as our request comes in, the request can be inspected 
        //and then the framework can decide whether or not to 
        //let the user proceed based on their authentication, token or otherwise.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    //get bytes format and then we need to go to our configuration to get the token keys
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                    //this gives our server enough information to take a look at the token
                    //and invalidate it just based on the issuer signing key which we have implemented.
                };
            });

            //To add the policy for authorization, refers to the AdminController
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });


        return services;
    }
}