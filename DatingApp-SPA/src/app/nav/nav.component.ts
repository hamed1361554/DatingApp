import { AlertifyService } from './../services/alertify.service';
import { AuthService } from './../services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  currentUserName: string;

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.currentUserName = this.authService.decodedToken?.unique_name;
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Logged in successfully');
      this.currentUserName = this.authService.decodedToken?.unique_name;
    }, error => {
      this.alertify.error('Failed to login: ' + error);
    });
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    this.alertify.message('Logged out successfully');
  }

}
