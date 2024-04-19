import { Component, Input } from '@angular/core';
import { Member } from '../../_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent {
  //To get the member info from our parent component (member-list)
  //Set the initial value of the member to be undefined(the values come later)
  @Input() member: Member | undefined;

}