import { User } from './../models/user';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient) { }

changeMemberPhoto(photoUrl: string) {
  this.photoUrl.next(photoUrl);
}

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model).pipe(
    map((response: any) => {
      const user = response;
      if (user) {
        localStorage.setItem('token', user.token);
        this.decodedToken = this.jwtHelper.decodeToken(user.token);
        localStorage.setItem('user', JSON.stringify(user.info));
        this.currentUser = user.info;
        this.changeMemberPhoto(this.currentUser.photoUrl);
      }
    })
  );
}

register(user: User) {
  return this.http.post(this.baseUrl + 'register', user);
}

loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

loadAlreadyLoggedInToken() {
  const token = localStorage.getItem('token');
  if (token) {
    this.decodedToken = this.jwtHelper.decodeToken(token);
  }

  const user: User = JSON.parse(localStorage.getItem('user'));
  if (user) {
    this.currentUser = user;
    this.changeMemberPhoto(user.photoUrl);
  }
}

unloadLoggedInInfo() {
  this.decodedToken = null;
  this.currentUser = null;
}

}
