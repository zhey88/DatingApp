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
    }
}

