import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { Observable } from 'rxjs';


@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})

export class MemberListComponent implements OnInit {
  //create a class property to store our members in
  members$: Observable<Member[]> | undefined

  //Inject memberService
  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    //get the list of members by calling the getMembers in memberService 
    this.members$ = this.memberService.getMembers();
  }

}