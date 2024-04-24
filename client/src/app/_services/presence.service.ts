import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection; //  //type of hub connection that we get from SignalR

//create an observable inside here because we want our components to subscribe to this
//information(when the user is online/offline) so that they're notified if something has changed
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) { }

  //To build up the connection for clients
  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
    //needs to match the name of what we called our endpoint when we mapped the hub in our program class
      .withUrl(this.hubUrl + 'presence', {
        //Use of accessTokenFactory to pass the user token
        accessTokenFactory: () => user.token
      })
      //our client, if it does lose a connection with our API server, is going to retry to connect to it
      .withAutomaticReconnect() //
      .build();

    //returns a promise that resolves when the connection has been successfully established or rejects
    this.hubConnection
      .start()
      .catch(error => console.log(error));

    //on method registers a handler that will be invoked when the hub method 
    //with the specified method name is invoked
    //To notified the other users that connected to the hub when the current user is online
    //when this method is received, we're going to update our online users with the newly online
    //or take the user away if they've just became offline
    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        //Update the list with the new username 
        next: usernames => this.onlineUsersSource.next([...usernames, username])
      })
    })

    //To notified the other users that connected to the hub when the current user is offline
    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe({
        // use the filter method, which also returns a new array, removed the username(offline)
        next: usernames => this.onlineUsersSource.next([...usernames.filter(x => x !== username)])
      })
    })

    //To call the method in the PresenceHub.cs, to get the information of currently online users
    this.hubConnection.on('GetOnlineUsers', usernames => {
      this.onlineUsersSource.next(usernames);
    })

    //To allow the user to send the message from the presenceHub to a user who is not online
    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toastr.info(knownAs + ' has sent you a new message! Click me to see it!')
      //When the click the toast, they will be navigated to the message tab
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=Messages'))
    })
  }

  //To stop the connection
  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}

