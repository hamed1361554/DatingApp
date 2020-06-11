import { AlertifyService } from './../services/alertify.service';
import { AuthService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  currentUserName: string;
  currentUserAvatar: string;

  constructor(private authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.currentUserName = this.authService.decodedToken?.unique_name;
    this.currentUserAvatar = this.authService.currentUser?.photoUrl;
    this.authService.currentPhotoUrl.subscribe(p => this.currentUserAvatar = p);
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Logged in successfully');
      this.currentUserName = this.authService.decodedToken?.unique_name;
      this.currentUserAvatar = this.authService.currentUser?.photoUrl;
    }, error => {
      this.alertify.error('Failed to login: ' + error);
    }, () => {
      this.router.navigate(['/members']);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.unloadLoggedInInfo();
    this.alertify.message('Logged out successfully');
    this.router.navigate(['/home']);
  }

}
