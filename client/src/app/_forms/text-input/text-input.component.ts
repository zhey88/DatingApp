import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
//allows us to write values, register when the input is changed and 
//register when the input has been touched.
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';

  //When we inject something into a constructor, it's going to check to see if it's been used recently
  //and if it has, it's going to reuse that thing that it's kept in memory
  //Now when it comes to our inputs, we do not want to reuse another control that was already in memory
  //We want to make sure that this  NgControl is unique to the inputs that we're updating in the DOM
  constructor(@Self() public ngControl: NgControl) { 
    //this represents our text input component class
    this.ngControl.valueAccessor = this;
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

  //we're effectively casting this into a form controlled to get around the TypeScript error that we
  //were seeing, can access this control simply by using what we've called it here control
  get control(): FormControl {
    return this.ngControl.control as FormControl
  }
}