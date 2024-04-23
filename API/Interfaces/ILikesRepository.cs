using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{

    public interface ILikesRepository
    {
        //To get the users that is being liked by the source user
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        //predicates, we mean do they want to get the user they liked or the user they are liked by
        //likes params, which includes our pagination params and the UserId and Predicate
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}