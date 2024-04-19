import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

//To prevent the user from leaving the page before saving the changes
export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  //Check if the user has made some changes to their profile
  if (component.editForm?.dirty) {
    return confirm('Are you sure you want to continue? Any unsaved changes will be lost')
  }

  return true;
};