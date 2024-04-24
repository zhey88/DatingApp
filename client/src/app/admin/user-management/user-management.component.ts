import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from '../../_models/user';
import { AdminService } from '../../_services/admin.service';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})

//user management, which is going to allow the admin to decide which roles a user should belong to
export class UserManagementComponent implements OnInit {
  users: User[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  availableRoles = [
    'Admin',
    'Moderator',
    'Member'
  ]

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  //Call getUsersWithRoles method in the adminService
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe({
      next: users => this.users = users
    })
  }

  //pass that information of user & user roles to the BSModalRef when we open the modal
  //we have a user object inside our template so we can pass it to this method
  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        username: user.username,
        availableRoles: this.availableRoles,
        selectedRoles: [...user.roles]
      }
    }
    //To show our rolesModal components.
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    //an event that is fired when the modal behind the ref starts hiding
    this.bsModalRef.onHide?.subscribe({
      next: () => {
        //when the model's hidden, we only want to do something if the user has actually updated the roles
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        //we don't want to do is send a request to our API server 
        //If the roles are exactly the same as in the users
        //call the arrayEqual method to compare the selectedRoles and the user.roles
        if (!this.arrayEqual(selectedRoles, user.roles)) {
          this.adminService.updateUserRoles(user.username, selectedRoles!).subscribe({
            next: roles => user.roles = roles
          })
        }
      }
    })
  }

  //Convert the two arrays to string so we could compare them
  private arrayEqual(arr1: any, arr2: any) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort())
  }
}