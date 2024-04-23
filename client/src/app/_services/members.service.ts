import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of, map, take } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  //store the updated members here
  members: Member[] = [];
  //To store the caching of the members for pagination and filtering so we dont have to load again
  //get the values for a particular key.
  //going to be storing these in key value pairs
  //the value is going to be our paginated results
  memberCache = new Map();
  user?: User | null;
  userParams: UserParams | undefined;

  //Save the user params into the service, so to remember the user query
  constructor(private http: HttpClient, private accountService: AccountService) {
      //get the current user and populate that inside a user property as well as our user params
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user=> {
          if(user){
            this.userParams = new UserParams(user);
            this.user = user
          }
        }
      })
   }

  //To be used inside the component to remember the users queries
  getUserParams() {
    return this.userParams;
  }

  setUserParams(userParams: UserParams) {
    this.userParams = userParams;
  }

  //Reset the filter button
  resetUserParams() {
    if (this.user) {
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }


  //To store the caching of the members for pagination and filtering so we dont have to load again
  //need to store the query as a key and store the results as a value.
  //we need to store the results for every type of query 
  //or every type of query that's happened from that particular user
  getMembers(userParams: UserParams ) {
    //Get the response which contains info such as pageNo, pageSize and gender
    //each time that we go and get these results, then we're going to check to see 
    //if this query has been made before using that key
    //get the results inside this variable called response
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    //Check if we have the reponse
    if (response) return of(response);
    //Send the http request to get the Members
    //we will get an array of member
    //However, we will need to pass the token to be authorized 
    //Pass the token we get from the getHttpOptions method
    //Get the user parameters from the userParams.ts
    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    //Filters
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    //when we go out and we do not have something in our member cache for this particular
    //query, then we're going to go to our API and with the results we get back, 
    //we're going to set them inside the member cache
    return this.getPaginatedResult<Member []>(this.baseUrl + 'users', params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    )
  }

  //Send the http request to get a single member with the username
  //Check if we have any members in the member array, if we do, just return the member 
  //Stroing the member detail into the memory, so we do not have to load them again
  getMember(username: string) {
    //This will store the memberDetails into the memberArray in the memory
    const member = [...this.memberCache.values()]
    //reduce method to combine the members array together into a single array
    //arr is the previous value(previous member detail you visited), 
    //elem is the current value(current member detail you looking at), store them into an empty array
      .reduce((arr, elem) => arr.concat(elem.result), [])
      //find method to avoid the duplicate users when using different queries
      .find((member: Member) => member.userName === username);

    if (member) return of(member);

    //If we do not have it in our cache, we simply going to go to the API and get to the user.
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
  

  //We need to get access to the full HTTP response because 
  //that's where our pagination header is residing
  //to observe the response to have access to the response
  //use of <T> to make this method resuable 
  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>;
    return this.http.get<T>(url , { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        // specify pagination as the header that we're interested in getting
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          //to get the serialized JSON into an object
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })

    );
  }

  private getPaginationHeaders(pageNumber : number, pageSize: number) {
    let params = new HttpParams();

      // we're going to set our params so that we can add the query string
      //that goes along with request
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);

    return params;
  }
}


