import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  constructor() {}

  ngOnInit(): void {
  }

  //To toggle the button
  registerToggle() {
    this.registerMode = !this.registerMode
  }


  //To receive the event emitted from register.component
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }

}