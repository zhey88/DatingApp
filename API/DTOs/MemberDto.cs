namespace API.DTOs
{
    //Use of the DTO for Members to specify what are the data exactly we want to return. 
    //We do not wish to include password hash and password salt. 
    //Convert date of birth into age 
    public class MemberDto
    {
            public int Id { get; set; }
            public string UserName { get; set; }
            //PhotoUrl will be the Url of the mainPhoto of a user
            public string PhotoUrl { get; set; }
            //Age will be linked with the GetAge method in AppUser.cs by automapper
            public int Age { get; set; }
            public string KnownAs { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastActive { get; set; }
            public string Gender { get; set; }
            public string Introduction { get; set; }
            public string LookingFor { get; set; }
            public string Interests { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public List<PhotoDto> Photos { get; set; }
    }
}
