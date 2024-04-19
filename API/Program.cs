//using System.Text;
using API.Data;
using API.Errors;
using API.Extensions;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//To add the Cors header to get the user data in our database
//need to modify our request on its way back to client and add the header
//builder.Services.AddCors();
//Call the services extensions in the ApplicationServices and IdentityServices
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

//exception handling has to go at the very top of the HTTP request pipeline
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
//Basically, with this, we are allowing the http request to access to the dabase from the 4200
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
//The middleware to authenticate the request for the AddAuthentication service
//UseAuthentication ask do you have a valid token?
//UseAuthorization, you have a valid token and what are you allowed to do?
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//this is going to give us access to all of the services that we have inside this program class
using var scope = app.Services.CreateScope();
var services  = scope.ServiceProvider;
//This is not a http request, so its not going to go through http request pipeline
//Hence, we do need to handle any exception over here
try
{
    var context = services.GetRequiredService<DataContext>();
    //this migrateAsync method asynchronously applies any pending migrations for the context to the database
    //and it will create the database if it does not already exist
    //So if we drop our database and we want to reset everything, all we need to do is literally drop the
    //database and restart our API
    //It will help us generate a new database, with new tables,new columns etc if something went wrong
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}


app.Run();
