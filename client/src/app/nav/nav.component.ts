import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent implements OnInit{

  model: any = {}


  //Make it to public so html file could use the currentUser$ directly
  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
  }

  //We should use a better way, Async pipe which we do not need to unsubscribe
  //To see if the user is currently logged in 
  //getCurrentUser(){
    //this.accountService.currentUser$.subscribe({
      //If there is a user, return true, else return false
      //next: user=> this.loggedIn = !!user,
      //error: error=>console.log(error)
    //})
 // }

  //If the user trying to login, we will take the username and password from the model
  //We need to subscribe the observable and make use of the observable
  //For http request, it is not essential to unsubscribe 
  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: error=> console.log(error)
    })
  }

  //Set the loggedIn to be false to show that the user already sign out
  logout(){
    //Call the method in accountService to Remove the user info from the localStorage 
    this.accountService.logout();
  }

}
