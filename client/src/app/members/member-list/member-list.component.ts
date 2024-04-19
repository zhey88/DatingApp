import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';


@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})

export class MemberListComponent implements OnInit {
  //create a class property to store our members in
  members: Member[] = [];

  //Inject memberService
  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    //as soon as we activate this root, then when it's initialized, it's going to call the load members
    this.loadMembers();
  }

  //we need to subscribe the observable 
  //get a list of members as our response
  loadMembers() {
    this.memberService.getMembers().subscribe({
      //call this argument members, which is going to be passed to this function
      //let the list of members = members
      next: members => this.members = members
    })
  }
}