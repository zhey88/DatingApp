﻿using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

//Make it static so that we could use the methods inside it without instantiating 
//a new instance of this class, we can just use it
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
                                            IConfiguration config)
    {
        //We need to specify the type of thing we want it to be
        //When we want to get something from database, we need the access to that DB Context Class, use a service
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        
        services.AddCors();
        //build the service to be lifetime for the token
        services.AddScoped<ITokenService, TokenService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        //Call the LogUserActivity to update the last active property inside the user
        services.AddScoped<LogUserActivity>();
        services.AddSignalR();
        //Did not use AddScoped, use Singleton so that
        //this dictionary to be available application wide 
        //for every user that connects to our service
        //Also, it will not be destoryed once the HTTP request has been completed
        services.AddSingleton<PresenceTracker>();
        //UnitOfWork will get all the repositories 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}