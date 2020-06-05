import { catchError } from 'rxjs/operators';
import { AlertifyService } from './../services/alertify.service';
import { UserService } from './../services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { User } from '../models/user';
import { Observable, of } from 'rxjs';

@Injectable()
export class MembersResolver implements Resolve<User[]> {
  constructor(private userService: UserService, private router: Router, private alertify: AlertifyService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): User[] | Observable<User[]> | Promise<User[]> {
    return this.userService.getUsers().pipe(
      catchError(error => {
        this.alertify.error('Problem retrieving users: ' + error);
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
