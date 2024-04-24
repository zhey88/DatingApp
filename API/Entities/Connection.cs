//To track the connections inside the Group

﻿namespace API.Entities
{
    //empty constructor so that when it does create this new connection,
    //it's not expecting to be able to also pass the connection ID as well
    public class Connection
    {
        public Connection()
        {

        }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }

}