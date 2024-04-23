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
            //use of unique name(TokenService.cs) instead of Identifiercomes in 
            //as when we're using this claim types
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        //extending the claims principle user object
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }

}