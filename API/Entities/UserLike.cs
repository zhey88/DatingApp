namespace API.Entities
{
    //This is going to create a join table between our app user and this entity
    //allow us to easily query a list of users that this user has liked and a user is liked by other users
    public class UserLike
    {
        //To like the target user
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        //To be liked by the source user
        public AppUser TargetUser { get; set; }
        public int TargetUserId { get; set; }
    }
}