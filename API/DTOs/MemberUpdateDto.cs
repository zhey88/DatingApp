﻿namespace API.DTOs
{
    //create a new DTO so that we can receive the information from the client
    //use this in the autoMapper so we dont have to do manual mapping
    public class MemberUpdateDto
    {
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}