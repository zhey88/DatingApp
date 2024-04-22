﻿using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    
    public AccountController(DataContext context, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _context = context;
    }

    //Post request to send the user details to the database
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        //Return error message when the username is created
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        //use of using to dispose the memory after using a class
        //HMACSHA512 just a way to hashmap the password
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            //Save the username into our database in lowercase for compare
            UserName = registerDto.Username.ToLower(),
             //To transform the hashmap password into bytes format
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key //The hashmap method will generate a random key and we could store it into salt
        };

        //Track the entities in the memory
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        //Return the response with the username and the token
        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            //For adding the mainphoto of the user to the nav bar
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url            
        };
    }

    //Allow the user to login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        //To get a hold of the userdata 
        //SingleOrDefaultAsync to find the value of the user, or return default value if is empty
        var user = await _context.Users
        // To include the user photo for adding photo onto the nav bar
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
        //If the user is not in the database, show error message
        if (user == null) return Unauthorized("Invalid username");

        //To check the password 
        //If we hash the PasswordSalt, it will get the same as the passwordHash
        using var hmac = new HMACSHA512(user.PasswordSalt);
        //To hashmap the password that user used to login 
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        //Compare the elements in the both of hash array, hmac and computedHash
        //If not equal, return invalid password
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        //If password matches return the user
        //Return the response with the username and the token
        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

//Method to check if the username is already created in our database
    private async Task<bool> UserExists(string username)
    {
        //x refers to the app user in the table, and loop over every user to determine
        //if what we looking for is exists
        //if exists, return true, else return false
        //Tolower to compare the username, also save the username in lowercase

        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}