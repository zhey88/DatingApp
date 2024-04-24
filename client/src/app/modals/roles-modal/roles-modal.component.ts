import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})

//goal here is to populate the properties inside our roles, modal component from our user management comp
//pass this from our user management component into our roles modal component
export class RolesModalComponent {
  username = '';
  availableRoles: any[] = [];
  selectedRoles: any[] = [];

  constructor(public bsModalRef: BsModalRef) { }

  //to change the values of whether a check box is checked or not checked
  //based on which role the users in
  updateChecked(checkedValue: string) {
    const index = this.selectedRoles.indexOf(checkedValue);
    //if the index is equal to minus one, that means it's not inside the selected roles array
    //And if it's not inside the selected roles array and it's been checked, 
    //then we want to add it to the selected roles, Otherwise we want to remove it
    index !== -1 ? this.selectedRoles.splice(index, 1) : this.selectedRoles.push(checkedValue);
  }

}