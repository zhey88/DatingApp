import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  //Inject accountService and toastrService
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  //If the user is logged in, we allow the user to procced, else pop out a error message
  return accountService.currentUser$.pipe(
    map(user => {
      if (user) return true;
      else {
        toastr.error('you shall not pass!');
        return false;
      }
    })
  )
};