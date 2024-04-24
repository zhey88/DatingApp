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
            CreateMap<RegisterDto, AppUser>(); //map the registerDto to the AppUser
            ////set up auto mapper so that we can map from our message to our message dto
            CreateMap<Message, MessageDto>()
            //Map the PhotoUrl, the rest of properties, Id, sendername, etc will be mapped by the framework
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
                    .FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos
                    .FirstOrDefault(x => x.IsMain).Url));

            //we use auto mapper to convert the date from what it is now into a UTC date.
            //So we could get the consistent date
            //creating mapping for utcNow, MessageSent, in the Message.cs
            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            //create another mapping for optional date times, DateRead
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? 
                DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
        }
    }

}