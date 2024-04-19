import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  //viewChild allows us to get access to the form values
  @ViewChild('editForm') editForm: NgForm | undefined;
  //if the form is dirty, then it's going to open up a browser prompt and ask them if they want to
  //leave the edit page before saving the updates
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  //Initialized the member profile to be undefined
  //this member will contain the updated information for that user
  member: Member | undefined;
  //this user contains the username and the token
  user: User | null | undefined;

  constructor(private accountService: AccountService, private memberService: MembersService,
    private toastr: ToastrService) {

    //get the user object in the localStorage with accountService
    //Use of pipe(take(1)) to automatically unsubscribe for us
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    });
  }

  ngOnInit(): void {
    this.loadMember(),
    console.log(this.member);
  }

  //load the member info
  loadMember() {
    //Check the user is login
    if (!this.user) return;
    this.memberService.getMember(this.user.username).subscribe({
      next: member => this.member = member
    
    })
  }

  updateMember() {
    //call the updateMember in the memberService and subscribe to the observable returned
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        //use a toastr to show a pop out message
        this.toastr.success('Profile updated successfully');
        //reset the form once the user click on the save changes button
        //when we reset, it will reset to the updated member's profile
        //this.member refers to the updated member array
        this.editForm?.reset(this.member);
      }
    })
  }
}