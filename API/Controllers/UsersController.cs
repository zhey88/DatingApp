using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    //Allow us to send our request to the client to hit the controller below and 
    //HttpGet 
    // The user only allow to send some request when they send us an authorized token
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        //Http request to get all the users in a list 
        [HttpGet] 
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            //Give us the list of users asynchronously
            //Call the getMembersAsync in the IUserRepository file
            //return OK to return our users as a list
            //we need to map the properties from MemberDto and PhotoDto, use of AutoMapper
            var users = await _userRepository.GetMembersAsync();
            //Specify the user info we want to return by mapping the properties in the MemberDto

            return Ok(users);
        }

        //Http request to get a single user of that username
        //Get the username of the user from the http address
        //Asyn allows the server to handle multiple requests at the same time
        [HttpGet("{username}")]  // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            //Give us the user information with the username
            //call the GetMmeberAsync in IUserRepository file
            //to Optimates the database query(stop quering the hash and salt password)
            return await _userRepository.GetMemberAsync(username); 
        }

         [HttpPut]
         //we're going to get the username from the token, because the user looks updating their own
         //profile, we'll have a token because this is going to be an authenticated request.
         //We don't need the username in the root parameter here, we can get that from the token
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //we do not need to return anything if the user updated their profile 
            //because it will be reflected to them directly
            //use the ? operator to prevent exception if the user is null
            //claim type Defines constants for the well-known claim types 
            //that can be assigned to a subject (NameIdentifier)
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Call the GetUserByUsernameAsync method in the IUserRepository to get the user info
            var user = await _userRepository.GetUserByUsernameAsync(username);
            
            //From our memberupdateDTO into our user when we retrieve this user from our repository
            //and Entity framework is now tracking this user and any updates to this user are going to be tracked by Entity framework
            //use of maper to update all of the properties that we pass through in that member
            //DTO into and overwriting the properties in that user, but it is not saved into the database yet
            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            //To save the data into the database, ensure 
            if (await _userRepository.SaveAllAsync()) return NoContent();
            //if theres no changes to the database, return a bad request
            return BadRequest("Failed to update user");
        }
        }
}

