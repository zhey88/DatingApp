import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.getUsers();
  }

  //To toggle the button
  registerToggle() {
    this.registerMode = !this.registerMode
  }

  getUsers() {
    //use of this to get the parameter from the AppComponent class
    //http request to get data from the database, server will return an observable
    //subscribe the observable to get the data
    this.http.get('http://localhost:5001/api/users').subscribe({
      //next, is to describe what we need to do when we get the data
      //use of call back function of what we want to do with returned data
      //() is the response and use of => to tell what we want to do with it
      next: response => this.users = response,
      error: () => console.log(Error),
      //when the request is completed
      complete: () => console.log('Request has completed')
    })
  }

  //To receive the event emitted from register.component
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

}