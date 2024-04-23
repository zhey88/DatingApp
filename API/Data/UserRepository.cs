﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
    //Inject the database
        private readonly DataContext _context;
        private readonly IMapper _mapper;

//going through these methods one by one and adding the codes to query our batabase as well
        public UserRepository(DataContext context
        , IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
        }

        //To return the user data base on the properties in MemberDto(without passwordHash and salt)
        //Optimates the database query(stop quering the hash and salt password)
        //Get the single user info with the given username
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
            //x referts to _context.Users
                .Where(x => x.UserName == username)
                //_mapper.ConfigureationProvider helps us to find the service which is in extensions.cs
                //When we use projection, we do not have to use Include eagerly load method to get the 
                //photoUrl entities
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        //Get the list of users
        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();

           //build up our query based on the information we have that we're
           //going to exclude the currently logged in user from the results that we return.
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            //ask about the gender
            query = query.Where(u => u.Gender == userParams.Gender);

            //To get the date of user borned between Max age and the Min age 
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            //filter the users that is between min age and max age
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            //To sort the users by the created and lastActive, newest user to oldest user
            query = userParams.OrderBy switch
            {
                //newest user to oldest user
                "created" => query.OrderByDescending(u => u.Created),
                //Most recently active user first
                _ => query.OrderByDescending(u => u.LastActive)
            };

            //Return the PagedList, CurrentPage,TotalPages,PageSize,TotalCount
            return await PagedList<MemberDto>.CreateAsync(query.AsNoTracking()
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                userParams.PageNumber, userParams.PageSize);
        }

        //all of these are tasks or returning a task of something
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            //To tell the API to include the user photos 
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username); // x refers to the appUser
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }
        
        //in order to return a boolean, we just want to make sure that the changes are greater than zero
        //If it's zero, then it's going to return false as in, nothing was saved into our database
        public async Task<bool> SaveAllAsync()
        {
            //method is part of the Entity Framework and is used to 
            //asynchronously save changes made in a database context to the underlying database
            return await _context.SaveChangesAsync() > 0;
        }

        //tells our Entity Framework Tracker that something has changed with the entity, the user
        //that we've passed in here, and we're not saving anything from this method at this point
        //We're just informing the Entity Framework Tracker that an entity has been updated.
        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}