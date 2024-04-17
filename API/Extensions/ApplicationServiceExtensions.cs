using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

//Make it static so that we could use the methods inside it without instantiating 
//a new instance of this class, we can just use it
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
                                            IConfiguration config)
    {
        //build the service to be lifetime for the token
        services.AddScoped<ITokenService, TokenService>();
        //We need to specify the type of thing we want it to be
        //When we want to get something from database, we need the access to that DB Context Class, use a service
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });


        return services;
    }
}