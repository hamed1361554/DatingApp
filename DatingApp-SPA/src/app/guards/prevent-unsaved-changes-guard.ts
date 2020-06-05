import { MemberEditComponent } from './../member-edit/member-edit.component';
import { Injectable } from '@angular/core';
import {
  CanDeactivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';

@Injectable()
export class PreventUnsavedChanges
  implements CanDeactivate<MemberEditComponent> {
  canDeactivate(
    component: MemberEditComponent,
    currentRoute: ActivatedRouteSnapshot,
    currentState: RouterStateSnapshot,
    nextState?: RouterStateSnapshot
  ):
    | boolean
    | UrlTree
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree> {
    if (component.editForm.dirty) {
      return confirm('Are you sure you want to continue? Any unsaved change will be lost!');
    }
    return true;
  }
}
