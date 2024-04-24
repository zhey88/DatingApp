namespace API.SignalR
{
    //tracking the presence, who is currently logged in to our Application
    //what we'll use is a dictionary to store key value pairs and we'll be able to 
    //store the connection ID of the user that's connected along with their username.
    //And we'll be able to maintain a list of users inside this dictionary about 
    //who is connected and who is not connected at any one given time.

    public class PresenceTracker
    {
        //First string will be userName, 
        //second string is a list of connection IDs for that particular user
        //Use of list for second string, because user can login using many different devices
        private static readonly Dictionary<string, List<string>> OnlineUsers = new(); //*********

        //Every time a user connects, they're given a unique connection ID
        //return a Boolean to specify if the user is genuinely just come online
        //As in they didn't have any other connections or live genuinely gone offline 
        //and we've removed all of their connections from the dictionary
        public Task<bool> UserConnected(string username, string connectionId)
        {
            bool isOnline = false;
            //going to lock our online users whilst we're adding the on connecting user to this dictionary
            //Anyone else that's connecting. They'll simply have to wait their turn to be added to this
            //if they've already got a connection and we're simply adding the connection ID to the key of username
            //then they haven't genuinely come online
            //They just added another connection and they were already online before
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    //And if they are already connected, then we're going to add their 
                    //new connection to our dictionary.
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    //If they do not already have a key inside our dictionary, 
                    //then we're going to create a new dictionary item
                    OnlineUsers.Add(username, new List<string> { connectionId });
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        //return a Boolean to specify if the user is genuinely just come online
        //As in they didn't have any other connections or live genuinely gone offline 
        //and we've removed all of their connections from the dictionary
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;
            lock (OnlineUsers)
            {
                //And if the username is not in our dictionary as a key, then we've got nothing to remove.
                //if we don't have that particular username in the dictionary, then the user is technically offline
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                //Stop the connection
                OnlineUsers[username].Remove(connectionId);

                //if the key of username count is zero, then we know the user has actually gone offline
                if (OnlineUsers[username].Count == 0)
                {
                //If we are removing the user connection from our dictionary and 
                //will set the is offline equal to true
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        //need a method inside here to actually get who is online so we can return that information
        //to other users that have logging into our application so that they can 
        //see who is online when they connect
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                //we get an alphabetical list of the users that are online and we're only 
                //interested in the key because that is the username
                //so that we get a list of those usernames of the users that are connected.
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
        //get the connections for a specific user as if they're connected
        //from their phone or tablet and their computer
        //We're going to send a notification to everyone of their connections that they've received a message
        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            //create a variable to store the list of string of the connection IDs
            List<string> connectionIds;

            lock (OnlineUsers)
            {
                //return a list of the connections for that particular user
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }

}