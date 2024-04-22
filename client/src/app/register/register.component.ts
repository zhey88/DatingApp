import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  //Emit something from child to parent(home.component) components, to call the cancelRegister method
  @Output() cancelRegister = new EventEmitter();
  //Reactive form method
  registerForm: FormGroup = new FormGroup({});
  //To calculate the age of the user
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    //when we pass this to our date picker, will only allow us to select up to a maximum date
    //Those user below 18 yrs cannot choose their dob
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18); 
  }

  //Initialize the form Properties with validators
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      //Make sure the password has minlength of 4 and max length of 8
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      //Make sure the password and the confirmPassword are the same
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });
    //To check the password and the confirmPassword is the same
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    });
  }

  //Custom validation to check if the two password is the same
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }


  //update the register function with reactive form
  register() {
    //GetDateOnly only gives us the date, remove the timezone information, get the dob from the form input
    const dob = this.GetDateOnly(this.registerForm.controls['dateOfBirth'].value);
    // to overwrite the value inside the registerForm.value for the date of birth
    const values = {...this.registerForm.value, dateOfBirth: this.GetDateOnly(dob)};
    //cal the register method in the accountService file
    this.accountService.register(values).subscribe({
      //after finish registering, navigate the user to members page 
      next: response => {
        this.router.navigateByUrl('/members');
      },
      error: error => {

        this.validationErrors = error;
      } 
    })
  //call the register method in the accountService with the userInput(this.model)
  //the response will be the username and the password

    //this.accountService.register(this.model).subscribe({
      //subscribe to the response, when it is completed, call the cancel method to cancel the form
     // next: () => {
       // this.cancel();
      //},
      //Use of the toastr to display a pop out erro message
      //error: error => {
       // this.toastr.error(error.error);
        //console.log(error);
      //}
    //})
  }

  //To turn the register mode in home component
  cancel() {
    this.cancelRegister.emit(false)
  }

  //Remove the timezone info and etc
  private GetDateOnly(dob: string | undefined) {
    if (!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).
      toISOString().slice(0,10); //Gives us only the date format
  }

}