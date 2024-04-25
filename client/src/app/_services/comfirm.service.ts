import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
//To give the user a pop out message
export class ConfirmService {
  bsModelRef?: BsModalRef<ConfirmDialogComponent>;

  constructor(private modalService: BsModalService) { }

  confirm(
    title = 'Confirmation', 
    message = 'Are you sure you want to do this?', 
    btnOkText = 'Ok', 
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    //if the results is true, as in the user has clicked okay on that button or it's going to
    //be an observable of a boolean that's going to be false if they clicked on decline.
      const config = {
        initialState: {
          title, 
          message,
          btnOkText,
          btnCancelText
        }
      }
      this.bsModelRef = this.modalService.show(ConfirmDialogComponent, config);
      //we want to do something when the modal is hidden
      //on hidden will return an observable and we want to project the result of this out.
      return this.bsModelRef.onHidden!.pipe(
        map(() => {
          return this.bsModelRef!.content!.result
        })
      )
  }
}