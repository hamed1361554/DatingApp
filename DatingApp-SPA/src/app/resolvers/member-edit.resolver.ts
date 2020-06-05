import { AuthService } from './../services/auth.service';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from './../services/alertify.service';
import { UserService } from './../services/user.service';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { User } from '../models/user';
import { Observable, of } from 'rxjs';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
  constructor(private userService: UserService, private authService: AuthService,
              private router: Router, private alertify: AlertifyService) {}

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): User | Observable<User> | Promise<User> {
    return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
      catchError(error => {
        this.alertify.error('Problem retrieving current user data: ' + error);
        this.router.navigate(['/members']);
        return of(null);
      })
    );
  }
}
