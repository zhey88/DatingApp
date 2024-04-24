import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { MessageService } from '../../_services/message.service';
import { Message } from '../../_models/message';


@Component({
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit {
  //To get the input from the message form
  @ViewChild('messageForm') messageForm?: NgForm;
  //Use of @Input() because this is a child component of the memberDetail component
  //So to get the properties of username and message[]
  @Input() username?: string;
  //For send message
  messageContent = '';

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
  }

  //For sending the message
  sendMessage() {
    if (!this.username) return;
    //because we're returning a promise now, we can use then instead of subscribe.
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset(); //reset the form
    })
  }

}