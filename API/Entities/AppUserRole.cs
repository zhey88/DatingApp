using Microsoft.AspNetCore.Identity;

namespace API.Entities
{

    //this is going to represent the join table between our app users and our roles
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }

}