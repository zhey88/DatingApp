using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        //Inject userManager to get users with roles
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //we can specify which role we want to allow to access this particular endpoint.
        //Refers to the AddAuthorization method in the IdentityServiceExtensions
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            //To get the list of users
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                //not only access the user roles table, but we also access the roles table
                .Select(u => new
                {
                    u.Id,
                    Username = u.UserName,
                    //Get the roleName from the role table
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        ////we can specify which role we want to allow to access this particular endpoint.
        ////Refers to the AddAuthorization method in the IdentityServiceExtensions
        ////To allow the Admin to edit the user roles
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")] //Should be HttpPut because we updating something, but we want to return the list
        //because we're using the username as root parameter and we're getting the roles from
        //the query string, we have to put [FromQuery] to tell it explicitly where to get this from
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            //check to see if we've got anything inside this query string
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
            //get this into an array because it's going to be a list of comma separated
            //values for each role the user's going to be inside of
            //Separate the username and roles by the ,
            var selectedRoles = roles.Split(",").ToArray();
            //get a hold of the user that we're trying to modify here
            //FindByNameAsync is the method inside the userManager
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();
            //get hold of the existing user roles that the user is inside of
            var userRoles = await _userManager.GetRolesAsync(user);
            //add the roles that the user is not currently in and add them to that role
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to add to roles");

            //For admin to remove the roles from the user
            //So any roles that the user was already inside of that are not contained 
            //inside the selected roles list, they're going to be removed from that particular role
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");
            //we return an updated list of roles that the user is in
            return Ok(await _userManager.GetRolesAsync(user));
        }


        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
    }
}