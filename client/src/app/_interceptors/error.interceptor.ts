import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()

//Intercepts and hanldes an HttpRequest or HttpResponse
//Allows us to intercept HTTP requests when they either go out to our API 
//and come back from our API and we're interested in what our API is returning 
//so that we can intercept these HTTP errors
//And depending on the type of error, we can handle it and spit out the 
//appropriate response what do we want to show to the client

export class ErrorInterceptor implements HttpInterceptor {
  //inject the router so we could redirect the user if we need to, depending on the error we get back
  //inject toastr to notify the user
  constructor(private router: Router, private toastr: ToastrService) {}

  //request for the httpRequest,  what happens next from the HTTP handler
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    //return an observable to client and we need to transform or midify an observable(use pipe)
    return next.handle(request).pipe(
      //catch any error in our interceptors
      catchError(error => {
        //If we have error
        if (error) {
          switch (error.status) {
            //we can have 2 different types of 400 error response
            case 400:
              if (error.error.errors) {
                //to store the errors
                const modelStateErrors = [];
                //to loop through the different errors in the array
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modelStateErrors.push(error.error.errors[key])
                  }
                }
                throw modelStateErrors.flat();
              } else {
                this.toastr.error(error.error, error.status);
              }
              break;
            case 401:
              this.toastr.error('Unauthorised', error.status);
              break;
            case 404: 
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        throw error;
      })
    )
  }
}