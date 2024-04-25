import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.css']
})
//To get the warning pop out box
export class ConfirmDialogComponent {
  title = '';
  message = '';
  btnOkText = '';
  btnCancelText = '';
  result = false;

  constructor(public bsModalRef: BsModalRef) {}

  //Hide the modal after click
  confirm() {
    this.result = true;
    this.bsModalRef.hide()
  }

  decline() {
    this.bsModalRef.hide();
  }

}