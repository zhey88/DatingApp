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
  @Input() messages: Message[] = [];
  //For send message
  messageContent = '';

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

  //For sending the message
  sendMessage() {
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message);
        //Clear the message form after sending the message
        this.messageForm?.reset();
      }
    })
  }

}