import { Component, Input } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  //To get the member info from our parent component (member-list)
  //Set the initial value of the member to be undefined(the values come later)
  @Input() member: Member | undefined;

  constructor(private memberService: MembersService, private toastr: ToastrService, 
    public presenceService: PresenceService) { }

  //Call the addLike method in the memberService to like the member and subscribe to the reponse
  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => this.toastr.success('You have liked ' + member.knownAs)
    })
  }
}