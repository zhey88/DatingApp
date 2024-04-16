using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]  //  /api/users
public class UsersController : ControllerBase
{
    private readonly DataContext _context;
    public UsersController(DataContext context)
    {
        //Same as this.context = context;
        _context = context;
    }

    //Http response to get all the users in a list 
    [HttpGet] 
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        //Give us the list of users asynchronously
        var users = await _context.Users.ToListAsync();

        return users;
    }

    //Http response to get a single user of that id
    //Get the id of the user from the http address
    //Asyn allows the server to handle multiple requests at the same time
    [HttpGet("{id}")]  // /api/users/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        //Give us the user information with the id
        return await _context.Users.FindAsync(id); 
    }
}


