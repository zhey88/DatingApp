import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';


//Create an interceptor to attach the token for all the requests sent to the api
//so we do not have to attach the token at the individual level(inside the memberService.ts)
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //use the currentUser observable, use pipe and take(1) method to auto unsubscribe for us
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      //what we want to do next: we will get a user from the observable
      //however, it could be a user or null, so we need to check that
      //if we have a user, take the request, clone the request
      //setHeaders method to attach the token
      next: user => {
        if (user) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
          })
        }
      }
    })
    //Move to the next request
    return next.handle(request);
  }
}