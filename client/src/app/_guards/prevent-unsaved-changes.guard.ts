import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/comfirm.service';

//To prevent the user from leaving the page before saving the changes
export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {

  const confirmService = inject(ConfirmService);
  //Check if the user has made some changes to their profile
  if (component.editForm?.dirty) {
    return confirmService.confirm();
  }

  return true;
};