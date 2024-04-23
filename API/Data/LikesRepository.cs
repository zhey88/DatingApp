using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{

    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        //Inject the database
        public LikesRepository(DataContext context)
        {
            _context = context;

        }

        //find the user like entity that matches the primary key
        //That's a combination of these two integers
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            //_context.Likes refers to the table Likes in the database
            //FindAsync Finds an entity with the given primary key values
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            //create a query for our users, get a list of our users in the database ordered by their username
            //But it's a queryable which means it has not been executed yet
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            //Get the likes
            var likes = _context.Likes.AsQueryable();

            //check to see if they want the users that they themselves have liked
            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                //use likes list to further refine and filter our variable here
                //only select the users that are inside likes list
                users = likes.Select(like => like.TargetUser);
            }

            ////check to see if they want the users that they themselves have liked by
            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            //give us a list of the users that are like to a like by depending on the predicates
            return await PagedList<LikeDto>.CreateAsync(likedUsers, 
                    likesParams.PageNumber, likesParams.PageSize);
        }



        //to allow us to check to see if a user already has been liked by another user
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            //To get the LikedUsers
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }

}