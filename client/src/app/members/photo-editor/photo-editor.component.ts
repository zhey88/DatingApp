import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from '../../_models/member';
import { Photo } from '../../_models/photo';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})

//This will be a child component of the member-edit component
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  //For the file upload set up
  uploader: FileUploader | undefined;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  //we need access to our user
  user: User | undefined;

  //We need to inject AccountService to accessto our user
  constructor(private accountService: AccountService, private memberService: MembersService) {
    //Unsubscribe when we complete the request
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user
      }
    })
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  //one of the methods we need to use so that we can get the drop zone functionality we need to provide
  fileOverBase(e: any) {
    this.hasBaseDropzoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    //Call the setMainPhoto method in the memberService to allow user to set their mainphoto
    //We need to subscribe because this is a HTTP request
    this.memberService.setMainPhoto(photo.id).subscribe({
      //What to do next
      next: _ => {
        //To check if we have the user and the member
        if (this.user && this.member) {
          //going to need to update the photo URL for that user
          this.user.photoUrl = photo.url;
          //Update the user info
          //when we update the main photo here, it's also going to update
          //any other component that's displaying that user's main photo
          this.accountService.setCurrentUser(this.user);
          //need to update the member as well because the member contains the photo collection
          //Because we using this mainphoto in the member detail and member card as well
          this.member.photoUrl = photo.url;
          //we need to update the elements inside the array here
          this.member.photos.forEach(p => {
            //we need to turn off the one that currently is main to false
            if (p.isMain) p.isMain = false;
            //we need to set the new photo to Main
            if (p.id === photo.id) p.isMain = true;
          })
        }
      }
    })
  }


  //Call deletePhoto in the memeberService to delete the photo
  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      //What we do next
      next: _ => {
        //Check if we have the member
        if (this.member) {
          //to return all the photos except for this photoId (!==photoId)
          this.member.photos = this.member?.photos.filter(x => x.id !== photoId)
        }
      }
    })
  }

  //To get the uploader functionality
  initializeUploader() {
    this.uploader = new FileUploader({
      //Where we going to send our image
      url: this.baseUrl + 'users/add-photo',
      //to specify user's token
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      //To allow all image types
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    }

    //What we going to do after we uploaded the images
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      //To check if we have a reponse
      if (response) {
        const photo = JSON.parse(response);
        //To add the images to the user
        this.member?.photos.push(photo);
        //after we've uploaded a photo successfully, we need to do the checks and see if the photo
        //is the main photo and we have this user and this member
        if (photo.isMain && this.user && this.member) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }
}