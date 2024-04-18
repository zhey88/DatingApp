import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  //Emit something from child to parent(home.component) components, to call the cancelRegister method
  @Output() cancelRegister = new EventEmitter();
  model: any = {};

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  //call the register method in the accountService with the userInput(this.model)
  //the response will be the username and the password
  register() {
    this.accountService.register(this.model).subscribe({
      //subscribe to the response, when it is completed, call the cancel method to cancel the form
      next: () => {
        this.cancel();
      },
      error: error => console.log(error)
    })
  }

  //To turn the register mode in home component
  cancel() {
    this.cancelRegister.emit(false)
  }

}