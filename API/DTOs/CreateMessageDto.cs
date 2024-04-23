namespace API.DTOs
{
    public class CreateMessageDto
    {
        //Who we sending the message to
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }

}