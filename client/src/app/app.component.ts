import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

//OnInit is a life cycle hook that runs after the constructor, to do additional initialization
export class AppComponent implements OnInit {

  title = "Dating App";
  //To store the users from the database
  users : any;

  //When AppComponent is instantiated, the constructor parameters is also created
  constructor(private http: HttpClient) {}

  ngOnInit(): void{

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

}
