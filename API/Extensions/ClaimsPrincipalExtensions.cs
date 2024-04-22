using System.Security.Claims;

namespace API.Extensions
{

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            //use the ? operator to prevent exception if the user is null
            //claim type Defines constants for the well-known claim types 
            //that can be assigned to a subject (NameIdentifier)
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

}