import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { TimeagoModule } from 'ngx-timeago';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';


@Component({
  selector: 'app-member-detail',
  //For standalone component, this member-detail.component is no longer needed to be 
  //provided in the app.module.ts, make this standalone so we could use ng gallery
  //Then we cannot use the imports in the app.module, hence we need to import here
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})

export class MemberDetailComponent implements OnInit {
  //get a hold of the member tabs inside this component
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  //this initializes our member with an empty object but 
  //should be populated from our route by the route resolver
  member: Member = {} as Member;
  //Save the images into the GalleryItem array
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute,
        private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next: params =>{
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages()
  }

  //Go to the message by clicking on the message button
  //If we have the member tabs, then we're pretty sure we're going to be able to find the heading
  selectTab(heading: string) {
    if (this.memberTabs) {
      //use of ! to turn off the typescript
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  //Only load the message detail when you click on it
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages') {
      this.loadMessages();
    }
  }

  loadMessages() {
    if (this.member)
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })


  }

  //we get the Image of the user
  getImages() {
    //We have the check if the user has logged in
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      //save the images into the array
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}));
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}));
    }
  }

}