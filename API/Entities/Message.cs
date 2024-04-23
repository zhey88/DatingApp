namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        // if a sender of a message decides to delete the message from their side
        //or their view of the messages, they don't have any control over the 
        //inbox of the of a user they send the message to
        //So we'll only actually physically remove the message from the database
        //If both of these values are set to true
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }

}