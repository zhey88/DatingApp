import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { User } from '../_models/user';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})

export class MessageService {
  baseUrl = environment.apiUrl;

  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();
  
  constructor(private http: HttpClient) { }

//create a hub connection inside here. Live chat
//we only want to connect when we're inside the messages tab of the member detail component.
//inside that message group so that we only get the private messaging between two users
  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
    //message as that's the name of the hub.
    //concatenate the other user name as this matches what we're doing in our API 
    //to get that user
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      //authenticate to this particular message hub
      //do automaticReconnect
      .withAutomaticReconnect()
      .build();
    //To start the hub connection
    this.hubConnection.start().catch(error => console.log(error));

    //Call the method ReceiveMessageThread in the MessageHub.cs to get the message back
    this.hubConnection.on('ReceiveMessageThread', messages => {
      //we're going to create an observable that's going to store these messages so that we
      //can subscribe to them inside our component.
      this.messageThreadSource.next(messages);
    })
 
    //we get a message back from our signal, our hub.
    //our goal here is to update what's inside messageThreadSource
    //We create a new array of messages, and we effectively replace the old array
    //to do that, we're going to need to access our existing messages inside this observable messageThread$
    //and use that, and then concatenate or add a new one on and replace 
    //the existing messages inside messageThreadSource
    this.hubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe({
        //we get the messages back from the message thread observable
        next: messages => {
          // use the spread operator, to create a new array to replace our existing array.
          //...messages is the existing messages, get replaced by the new messages, message
          this.messageThreadSource.next([...messages, message])
        }
      })
    })

    //the purpose of this is because if a user joins our group, then we are not going to receive the
    //message thread. So we are not going to know or our message thread is not going to be updated 
    //with the fact that they've now read the message
    //we're going to check inside this message thread to see if there's any messages from 
    //the user that's joined the group that are currently unread.
    //And if there are, we're just going to mark them as red on the client to match 
    //how they're going to be in our server
    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      //this is the person that's joining the group
      if (group.connections.some(x => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            //check to see if there's any unread messages, and mark them as read
            messages.forEach(message => {
              if (!message.dateRead) {
                message.dateRead = new Date(Date.now())
              }
            })
            //use the spot operator again so that we replace the array with our updated version array
            this.messageThreadSource.next([...messages]);
          }
        })
      }
    })
  }

  //To stop the hub connection
  stopHubConnection() {
    //Only stop the connection when we have connected
    if (this.hubConnection) {
      this.hubConnection?.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  //Send the message in the message box
  async sendMessage(username: string, content: string) {
    //invokes a message on our server, on our API hub
    //call the SendMessage method in the messageHub
    return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content })
        //invoke method will return a promise
      .catch(error => console.log(error));
  }

  //To delete the message with the message id
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}