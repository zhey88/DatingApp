using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        //To apply many to many relationship, 1 role can have many users, 1 users can have many roles
        public ICollection<AppUserRole> UserRoles { get; set; }
    }

}