//create a controller so that we can use the functionality 
//that we now have inside the LieksRepository
﻿using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        public LikesController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //As we're creating a new resource, when a user likes of a user that's going to 
        //be creating a new resource
        [HttpPost("{username}")] //specify the user name of the person they are about to like as a root parameter
        public async Task<ActionResult> AddLike(string username) //To update our join table
        {
            //this is the user that's going to be liking other user, 
            //call the GetUserId in the ClaimsPrincipleExtensions
            var sourceUserId = User.GetUserId();
            var likedUser = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _uow.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();
            //Disable the user to like themselves
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            //To get the user that is been liked by the source user
            var userLike = await _uow.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            //Pass the info
            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            //create entry to user likes table, save the like user into the database
            sourceUser.LikedUsers.Add(userLike);

            //Save the changes in the database
            if (await _uow.Complete()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        //LikesParams contains all the properties in the PaginationParams and the userId and predicate
        //Use of [FromQuery] to find the likesParams
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes
            ([FromQuery]LikesParams likesParams)
        {
            
            likesParams.UserId = User.GetUserId();

            //using predicate, so we can choose whether or not to 
            //send back the liked users or liked by users
            var users = await _uow.LikesRepository.GetUserLikes(likesParams);
            //we're getting a pagination header
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
                users.TotalCount, users.TotalPages));

            return Ok(users);
        }
    }

}