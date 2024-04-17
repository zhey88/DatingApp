using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//We need to specify the type of thing we want it to be
//When we want to get something from database, we need the access to that DB Context Class, use a service
builder.Services.AddDbContext<DataContext>(opt=>
{
    //Configure the context to connect to a SQLite database
    //ConnectionString will be set in the Program.cs file
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//To add the Cors header to get the user data in our database
//need to modify our request on its way back to client and add the header
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
//Basically, with this, we are allowing the http request to access to the dabase from the 4200
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
app.MapControllers();

app.Run();
