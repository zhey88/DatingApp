using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{

    //Use the dtos allows us to create an object which sends back the properties
    // that we are interested (we do not want to show the hashpassword and saltpassword)
    public class RegisterDto
    {
        //Make the username and password fields to be required
        [Required]
        public string Username {get; set;}
        [Required]
        //Set the max length of the password to be 8 and min length to be 4
        [StringLength(8, MinimumLength = 4)]
        public string Password {get; set;}
    }
}