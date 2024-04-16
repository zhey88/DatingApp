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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
