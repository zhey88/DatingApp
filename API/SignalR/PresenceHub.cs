//to display the presence of a user in our application
//to show whether the user is online or not

﻿using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{


[Authorize]//Only allows the authenticated user to connect to hub
//inside our hub we can override a couple of methods and one of them is when a user connects to this
//hub, and the other one is when a user disconnects from the hub
public class PresenceHub : Hub      
{
    private readonly PresenceTracker _tracker;
    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }

    //When a user connects to this hub
    public override async Task OnConnectedAsync()
    {
        //To track the users who are currently online
        var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        //we're going to send a message to other users to notify
        //we're only going to notify the clients that are connected 
        if (isOnline)
        //when this user does connect, anybody else that's connected to this same hub is 
        //going to receive the user name that has just connected
        //Client can be used to invoke methods on clients that are connected to this hub
        //if the user has just come online based on the logic we have inside our presence tracker
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

        //get a list of the currently online users
        var currentUsers = await _tracker.GetOnlineUsers();
        //send this list of currently login users to all of the connected clients when somebody connects
        //allow clients connected to our application to update their list of who is currently
        //online so that we can display that information in the browser
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    ////When a user disconnects from this hub
    public override async Task OnDisconnectedAsync(Exception ex)
    {
        //To track the users who are disconnected
        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

        if (isOffline)
        //Only then will we notify the other clients that the user has gone offline
        //when this user disconnect, anybody else that's connected to this same hub is 
        //going to receive the user name that has just disconnected

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());


        await base.OnDisconnectedAsync(ex);
    }
}

}
