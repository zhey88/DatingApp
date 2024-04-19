import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { BusyService } from '../_services/busy.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy(); //Whenever we sending a request, we call the busy method to show the spinner

    //To fake some kind of delay in our application to pretend that our requests are going to take a second.
    return next.handle(request).pipe(
      delay(1000),
      //finalize() method is what we do do after things have completed
      finalize(() => {
        this.busyService.idle(); // to call the idle method to hide the spinner
      })
    )
  }
}