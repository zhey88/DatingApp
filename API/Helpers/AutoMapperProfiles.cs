using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    //We need to use automapper to link the properties in the MemberDtos and PhotoDto 
    //to specify what kinds of data we wish to return when the user send a request to get the users
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //we need to tell auto mapper what we want to go from and what we want to go to
            //go from AppUser to MemberDto
            CreateMap<AppUser, MemberDto>()
            //To configure an individual property, for the PhotoUrl to be display(user's main photo)
            //Specify the detination of the member PhotoUrl and the option(what do want to d)
                .ForMember(dest => dest.PhotoUrl, opt =>
                //We want to map something, map from src.Photos, the source that map with the property
                    opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                    //to map the age to CalculateAge method in the DateTimeExtensions.cs
                    //The AutoMapper could able to link Method to the properties age(int) 
                    //In the MemberDto.cs
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();  //go from AppUser to MemberDto
            //We do not need to add any additional configuration here because all the properties are the same
            CreateMap<MemberUpdateDto, AppUser>(); //map the updatedMember details to the AppUser
        }
    }

}