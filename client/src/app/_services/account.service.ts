import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';
import { PresenceService } from './presence.service';

//Same as put this inside the provider array in app.module
@Injectable({
  providedIn: 'root'
})

//This service will be responsible for making the HTTP requests from our client to our server
export class AccountService {
  //Get the apiUrl from the environment file
  baseUrl = environment.apiUrl;
  //The behaviorSubject allows us to give an observable an initial value(we set it to be null)we that 
  //then can use in other components
  private currentUserSource = new BehaviorSubject<User | null>(null);
  //The $ just to show that it is an observable
  currentUser$ = this.currentUserSource.asObservable();

  //Automatically allow the user to connect to the hub when we set the user
  constructor(private http:HttpClient, private presenceService: PresenceService) { }

  login(model: any)
  {
    //It will return an observable with the response
    //We going to return our token and the username from the API server 
    //save the uesrname and the token in user
    //<User> is to set the format of the response we should get, user.ts
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      //The user comes from the user.ts, interface
      map((response: User) => {
        const user = response;
        //If we have a user and going to save the user info into the local storage
        //change the format of the info from json into string object
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  register(model: any) {
    //Send a http request to post the user input when the user is at account/register form
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(response => {
        //save the http response in to user
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }
  
//set this information inside our account service.
//Call this method whenever we login or we register, or when the user refresh the browser, 
//when we get token from local storage
  setCurrentUser(user: User) {
    user.roles = [];
    //Call getDecodedToken method to get the roles of the user
    const roles = this.getDecodedToken(user.token).role;
    //going to check to see if this is an array, yes --> add multiple roles, no --> add single role
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    //To let the other components know about the login user informaiton
    //to update our current user source with the user if we successfully log in.
    this.currentUserSource.next(user);
    //save the register user info into the local storage
    localStorage.setItem('user', JSON.stringify(user));
    //Build the hub connection whenever we call this method
    this.presenceService.createHubConnection(user);
  }

  //Remove the information from the localStorage after we logout 
  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    //Stop the connection when we logout
    this.presenceService.stopHubConnection();
  }

  //split by the period that's inside our token and we're only interested in getting the middle parts
  //which contains id, roles and the username
  //For admin guard, restrict users that are not admin role to access admin page
  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
