using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //Allow us to send our request to the client to hit the controller below and 
    //HttpGet 
    // The user only allow to send some request when they send us an authorized token
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _uow = uow;
            _mapper = mapper;
        }

        //we then specify which roles we want to allow to access this particular endpoint.
        //[Authorize(Roles="Admin")]

        //Http request to get all the users in a list 
        //Now we're going to ask the client to send this up as a query string
        //when we use a query string, we're going to need to tell our API where to find it
        //[FromQuery] where it needs to look to find these userParams
        [HttpGet] 
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers(
                                [FromQuery] UserParams userParams)
        {
            //To get the current user gender
            var gener = await _uow.UserRepository.GetUserGender(User.GetUsername());
            //we're going to get our current user username to populate that particular field
            userParams.CurrentUsername = User.GetUsername();

            //Now we want of course our users to be able to select which gender they want to view, but if they do
            //not make a selection or they've just loaded up the member's page, then we're going to send back a default
            // check to see if the string dot is null or empty for the user params gender
            if (string.IsNullOrEmpty(userParams.Gender))
                //check if the user is male 
                //if is male, we going to set the gender to female
                userParams.Gender = gener == "male" ? "female" : "male";

            //Give us the list of users asynchronously
            //Call the getMembersAsync in the IUserRepository file
            //return OK to return our users as a list
            //we need to map the properties from MemberDto and PhotoDto, use of AutoMapper
            var users = await _uow.UserRepository.GetMembersAsync(userParams);
            //Specify the user info we want to return by mapping the properties in the MemberDto

            //Now we also want to return our pagination information via pagination header
            //So we're going to get access to our HTTP response inside our API controller
            //created an extension method called add pagination header
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages));

            return Ok(users);
        }

        //[Authorize(Roles="Member")]
        
        //Http request to get a single user of that username
        //Get the username of the user from the http address
        //Asyn allows the server to handle multiple requests at the same time
        [HttpGet("{username}")]  // /api/users/2
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            //Give us the user information with the username
            //call the GetMmeberAsync in IUserRepository file
            //to Optimates the database query(stop quering the hash and salt password)
            return await _uow.UserRepository.GetMemberAsync(username); 
        }

         [HttpPut] //An update response, to update the users info
         //we're going to get the username from the token, because the user looks updating their own
         //profile, we'll have a token because this is going to be an authenticated request.
         //We don't need the username in the root parameter here, we can get that from the token
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            //we do not need to return anything if the user updated their profile 
            //because it will be reflected to them directly
            //Call the GetUserByUsernameAsync method in the IUserRepository to get the user info
            var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            
            //From our memberupdateDTO into our user when we retrieve this user from our repository
            //and Entity framework is now tracking this user and any updates to this user are going to be tracked by Entity framework
            //use of maper to update all of the properties that we pass through in that member
            //DTO into and overwriting the properties in that user, but it is not saved into the database yet
            _mapper.Map(memberUpdateDto, user);

            //_uow.UserRepository.Update(user);

            //To save the data into the database, ensure 
            if (await _uow.Complete()) return NoContent();
            //if theres no changes to the database, return a bad request
            return BadRequest("Failed to update user");
        }

    //To upload the images onto the cloudinary 
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        //Check if theres a user
        if (user == null) return NotFound();

        //Call the AddPhotoAsync in the IPhotoService to add a new photo to the user
        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            //The path of the image
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        //if this photo is the first photo of the user, make this the main photo 
        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _uow.Complete())
        {
            //To tell the API where to find the newly created resource, 
            //refer to the GetUser method to map the username and the photo
            return CreatedAtAction(nameof(GetUser), new { username = user.UserName },
            //Also, send back the newly created resources
                _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding photo");
    }

    //To Update the main photo of the user
    [HttpPut("set-main-photo/{photoId}")]  //route
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        //To get the user
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        //Make sure we have a user
        if (user == null) return NotFound();
        //Get hold of the photo from that user object
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        //To make sure there is a photo uploaded
        if (photo == null) return NotFound();
        //to check to see if this photo is already the user's main photo
        //If it is, we do not want to allow them to set it as main again        
        if (photo.IsMain) return BadRequest("This is already your main photo");
        //then we'll check to see what the current main photo is,
        //to need to switch that to be not the main photo
        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        //That means we already have a photo that is set to the main or has is main set to true
        if (currentMain != null) currentMain.IsMain = false;
        //then we can set the new photo or the photo we're updating is main equal to true
        photo.IsMain = true;

        if (await _uow.Complete()) return NoContent();

        return BadRequest("Problem setting main photo");
    }

    //To for deleting photo
    [HttpDelete("delete-photo/{photoId}")]
    //Do not need to return anyting, as the user could able to see
    public async Task<ActionResult> DeletePhoto(int photoId)
    {

        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        //To check if the photo selected is the main photo
        if (photo.IsMain) return BadRequest("You cannot delete your main photo");
        //To check if the photo has a public Id
        //If do not have, we cant delete the photo in the cloudinary
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }
        
        //To remove the photo
        user.Photos.Remove(photo);
        //To save the changes
        if (await _uow.Complete()) return Ok();

        return BadRequest("Problem deleting photo");
    }    
    }


        
}

