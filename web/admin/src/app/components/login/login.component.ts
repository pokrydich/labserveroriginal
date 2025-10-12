import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LoginService } from '../../services/API/login.service';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { PopupNotificationsService } from '../../popup-notifications.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, TranslateModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder,
    private loginService: LoginService,
    private notificationService: PopupNotificationsService) {
    this.loginForm = fb.group({
      'username': [''],
      'password': ['']
    });
  }

  /**
   * login via HTTP API
   */
  protected login() {
    const val = this.loginForm.value;
    if (val.username && val.password) {
      this.loginService.login(val.username, val.password);
    } else {
      this.notificationService.error('loginErrorEnterUsernameAndPassword');
    }
  }
}
