import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { Observable, take } from 'rxjs';
import { Pagination } from '../../_models/pagination';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { User } from '../../_models/user';


@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})

export class MemberListComponent implements OnInit {
  //create a class property to store our members in
  //members$: Observable<Member[]> | undefined

  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  //A dropdown list For the user to select what gender they wish to display
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }]

  //Inject memberService
  constructor(private memberService: MembersService) { 
    //Get the userParams from the memberService
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    //get the list of members by calling the getMembers in memberService 
    //this.members$ = this.memberService.getMembers();

    this.loadMembers();
  }

  //To get the members in the page number and size 
  loadMembers() {
    //Check if we have the userParams
    if (this.userParams) {
      //going to set them to the user params, whatever the user has selected
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }
  }


  //Reset the filter button
  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
    }

  //For navigate to next page and display the next pageSize of members
  pageChanged(event: any) {
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      //Use of the Serice to remember the user queries to change pages and filter
      this.memberService.setUserParams(this.userParams);
      this.userParams.pageNumber = event.page;
      this.loadMembers();
    }
    }


}