import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'  // *appHasRole = '["Admin", "Thing"]'
})


//Hide the admin link for users that has no access to it
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[] = [];
  user: User = {} as User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, 
    private accountService: AccountService) { 
      //set our user to our current user observable
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user) this.user = user;
        }
      })
    }

  ngOnInit(): void {
    //using this statement asking if the roles that we want to check for admin or thing
    //If the user roles, some of them at least match something inside this array that includes the role
    //then we're going to display the contents
    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      //If not contain the roles we want
      //clear the view containerRef, then we're removing that element from the DOM
      //Which means the admin link will be removed from the DOM
      this.viewContainerRef.clear();
    }
  }

}