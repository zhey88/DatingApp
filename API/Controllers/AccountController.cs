﻿using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    
    //use the user manager to manage our users
    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _mapper = mapper;
    }

    //Post request to send the user details to the database
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        //Return error message when the username is created
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        //we're going to go to our app user from our register data
        var user = _mapper.Map<AppUser>(registerDto);

        //Save the username into our database in lowercase for compare
        user.UserName = registerDto.Username.ToLower();
    
        //use a variable to store the result of this because we do get something back
        //from our user manager when we do create a new user
        //Save the user into our database
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        //check to see if the result has succeeded
        if (!result.Succeeded) return BadRequest(result.Errors);

        //when we register a new user, we'll take the opportunity and add them to the member role as that's
        var roleResult = await _userManager.AddToRoleAsync(user, "Member");
        if (!roleResult.Succeeded) return BadRequest(result.Errors);

        //Return the response with the username and the token
        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender             
        };
    }

    //Allow the user to login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //To get a hold of the userdata 
        //SingleOrDefaultAsync to find the value of the user, or return default value if is empty
        //Get the user info from database using userManager 
        var user = await _userManager.Users
        // To include the user photo for adding photo onto the nav bar
            .Include(p => p.Photos)
            //Get the user that matches the username
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
        //If the user is not in the database, show error message
        if (user == null) return Unauthorized("Invalid username");

        //Check if the password is correct, loginDto.Password returns a boolean value
        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result) return Unauthorized();

        //If password matches return the user
        //Return the response with the username and the token
        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            //For adding the mainphoto of the user to the nav bar
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender 

        };
    }

//Method to check if the username is already created in our database
    private async Task<bool> UserExists(string username)
    {
        //x refers to the app user in the table, and loop over every user to determine
        //if what we looking for is exists
        //if exists, return true, else return false
        //Tolower to compare the username, also save the username in lowercase
        //we can use our user manager instead of dbcontext to access our users
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}