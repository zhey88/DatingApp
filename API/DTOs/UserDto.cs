namespace API.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        //To add the main photo of the user to the navbar
        public string PhotoUrl { get; set; }      
        public string KnownAs { get; set; }  
    }
}