using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.SignalR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{

    //Authenticate to this message hub
    [Authorize]
    //For enable live chat
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        //To allow the user to receive the message when they online
        private readonly IHubContext<PresenceHub> _presenceHub;

        //we're going to want to return the message thread between 
        //them and the user they're connected to.
        public MessageHub(IUnitOfWork uow,IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _uow = uow;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            //when we do make a connection to a signal our hub, 
            //we do send up an HTTP request to initialize that connection
            //That gives us an opportunity to send something in a query string
            var httpContext = Context.GetHttpContext();
            //use of query to get the name of the user that we are connected to.
            var otherUser = httpContext.Request.Query["user"];
            //we need to put the users in a group, and the group name effectively is going to be a combination
            //of one username and the other username, call the GetGroupName 
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            //Add the users to the group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            //So the client that receives this can use this method to 
            //update the list of members in a particular message group
            //n this method we're updating the group so the client knows who's 
            //inside a group at any one given time
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            //Get the message thread between the 2 users
            //get the message thread from the repository
            var messages = await _uow.MessageRepository
                .GetMessageThread(Context.User.GetUsername(), otherUser);

            //use our unit of work and see if there are changes that need to be saved
            //if there are at this point, we can go ahead and save them
            if (_uow.HasChanges()) await _uow.Complete();
        
 

            //when a user connects to this message hub and they're going to do this on the context of the
            //memberdetail page, the messages tab, then they're going to receive the messages from SignalR
            //instead of making an API call to go and get them
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }


        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(ex);
        }

        //To build the live chat between the users
        //Also allow the user to send a message to a user that is not connected to the hub
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            //To get the username
            var username = Context.User.GetUsername();

            //Check if you sending message to yourself
            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException("Not found user");

            //create the message
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            //To get hold of the groupname by calling the method GetGroupName
            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            //now that we have the group name, we can go and get the group from our database
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);

            //then we can check our connections and see if we do have a username inside there 
            //that matches the recipient username. And if so, then we can mark the message as read
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            //if we do not have that message group
            // if there are any other parts of our application and not connected to the same message group
            //as the user that is sending the message, then we're going to 
            //allow them to receive a notification that they've had a new message.
            else
            {
                //Get the connection for a particular user
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if (connections != null) //Then we know the users connected to our application
                {
                    //use the presenceHub, to send a message to all of the clients connected 
                    //with whatever connection ID
                    //So if they're not connected, they're not going to receive any notification
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }
            //Add the message to the repository
            //in order for Entity Framework to track this, then we need to use the context.ADD
            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete())
            {
                //use of groupname as who we're sending this new message to
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        //got our group name with the two user names in alphabetical order.
        private string GetGroupName(string caller, string other)
        {
            //return a boolean by using less than zero in this case.
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        //add a group to our database
        //when we add to the group, we're going to return the group
        //itself so that we can see who is inside the group already and control 
        //who we send the message thread back to
        private async Task<Group> AddToGroup(string groupName)
        {
            //Get the group
            var group = await _uow.MessageRepository.GetMessageGroup(groupName);
            //create a new connection
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            //Check if we have any group
            if (group == null)
            {
                //use the new group that we've just created
                group = new Group(groupName);
                _uow.MessageRepository.AddGroup(group);
            }

            //Add the connection to the new group we just created
            group.Connections.Add(connection);

            //when we add to the group, we're going to return the group
            //itself so that we can see who is inside the group already
            if (await _uow.Complete()) return group;

            throw new HubException("Failed to add to group");
        }

        //Remove the connection from the group
        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _uow.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            //get this from the group because we included the connections with the group from our repository
            var connection = group.Connections
                .FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            //removing the connection from our database.
            _uow.MessageRepository.RemoveConnection(connection);

            if (await _uow.Complete()) return group;

            throw new HubException("Failed to remove from group");
        }
    }

}