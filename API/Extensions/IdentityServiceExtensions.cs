using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
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

//Now, in order for this service to be used, we need to add the 
//middleware to authenticate the request. After useCors and before MapControllers

        return services;
    }
}