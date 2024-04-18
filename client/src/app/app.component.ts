import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

//OnInit is a life cycle hook that runs after the constructor, to do additional initialization
export class AppComponent implements OnInit {

  title = "Dating App";

  //When AppComponent is instantiated, the constructor parameters is also created
  constructor(private accountService: AccountService) {}

  ngOnInit(): void{
    this.setCurrentUser();
  }


  //We get the user info from the localStorage, if theres no user, just return
  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    //Change the user to string format
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }

}
