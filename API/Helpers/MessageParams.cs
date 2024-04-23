namespace API.Helpers
{

    public class MessageParams : PaginationParams
    {
        public string Username { get; set; }

        //Container is to let user to decide which message they want to go to
        //Unread messages and Read messages
        public string Container { get; set; } = "Unread";
    }

}