import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-detail',
  //For standalone component, this member-detail.component is no longer needed to be 
  //provided in the app.module.ts, make this standalone so we could use ng gallery
  //Then we cannot use the imports in the app.module, hence we need to import here
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule]
})

export class MemberDetailComponent implements OnInit {
  member: Member | undefined;
  //Save the images into the GalleryItem array
  images: GalleryItem[] = [];

  constructor(private memberService: MembersService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
  }

  //To get the member details
  //Get the username from the current route
  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    //Check if the user is login
    if (!username) return;
    //If we have the user, we get the member info from the service 
    //base on the username we get from current route, we get the member info
    this.memberService.getMember(username).subscribe({
      next: member => {
        this.member = member,
        //call the getImages method, after we get the member, we get the image of the user
        this.getImages()
      }
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