import { Router } from '@angular/router';
import { AlertifyService } from './../services/alertify.service';
import { AuthService } from './../services/auth.service';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { User } from '../models/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();

  model: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(private authService: AuthService, private alertify: AlertifyService,
              private formBuilder: FormBuilder, private router: Router) { }

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    }
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['female'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      birthDate: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required,
        Validators.minLength(6),
        Validators.maxLength(32)]],
      confirmPassword: ['', Validators.required]
    }, {
      validators: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(g: FormGroup){
    return g.get('password').value === g.get('confirmPassword').value ? null : {mismatch: true};
  }

  register() {
    if (!this.registerForm.valid) { return; }

    this.model = Object.assign({}, this.registerForm.value);
    this.authService.register(this.model).subscribe(() => {
      this.alertify.success('Successful Registration');
    }, error => {
      this.alertify.error('Registration failed: ' + error);
    }, () => {
      this.authService.login(this.model).subscribe(() => {
        this.router.navigate(['/members']);
      });
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
