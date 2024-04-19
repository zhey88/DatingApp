import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})

//However, we need an interceptor to tell us when the http request is been sent and getting back
//add effectively an interceptor that's going to kick off that spinner 
//when an Http request is being made and stop it where the Http request has been received
//make use of the loadingInterceptor
export class BusyService {
  //as a request is taking place, then we're going to increase this busy count
  //whilst this busy count is greater than zero, we're going to display the spinner
  busyRequestCount = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  //If we are loading the page, increase the busy count
  busy() {
    this.busyRequestCount++;
    //show the spinner when the busy count is bigger than zero
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    })
  }

  ////If we are not loading the page, decrease the busy count
  idle() {
    //do not show the spinner when the busy count is lesser than zero
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}