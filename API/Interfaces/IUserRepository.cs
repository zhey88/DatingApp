﻿﻿using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        //IEnumerable is a type of list which supports a simple iteration over a collection of a specified type
        //this particular method is just to get our users
        Task<IEnumerable<AppUser>> GetUsersAsync();
        //Get a single user of that ID
        Task<AppUser> GetUserByIdAsync(int id);
        //Get a single user of that username
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto> GetMemberAsync(string username);

    }
}