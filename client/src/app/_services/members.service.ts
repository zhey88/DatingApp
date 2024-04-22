import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, map } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  //store the updated members here
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  //Send the http request to get the Members
  //we will get an array of member
  //However, we will need to pass the token to be authorized 
  //Pass the token we get from the getHttpOptions method
  getMembers() {
      //Check if we have any members in the member array, if we do, just return the member 
    //of indicates that we return an observable
      //use the map method to project what we're getting back from our API,list of members
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    )
  }

  
  //Send the http request to get a single member with the username
    //Check if we have any members in the member array, if we do, just return the member 
  getMember(username: string) {
    const member = this.members.find(x => x.userName === username);
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

    //Use of put request to updates an the current user information
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        //we will get the index of the array of that member
        //update the member of that index
        const index = this.members.indexOf(member);
        //... is a spread operator to stop members at that index
        //this spread operator takes all of the elements of the member at this location in the array
        //update those properties by using the spread operator again 
        //and updating it with what's inside this member
        this.members[index] = { ...this.members[index], ...member }
      })
    )
  }

  //To allow the users to set their own mainPhotos
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  //TO allow the user to delete the user
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }  
}


