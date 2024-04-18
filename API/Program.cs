using API.Extensions;
using API.Errors;

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

app.Run();
