import { HttpClient} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  //Get the apiUrl from the environment file
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  //Send the http request to get the Members
  //we will get an array of member
  //However, we will need to pass the token to be authorized 
  //Pass the token we get from the getHttpOptions method
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }
    

  //Send the http request to get a single member with the username
  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
}