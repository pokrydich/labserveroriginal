import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PopupNotificationsService } from '../../popup-notifications.service';
import { RegisterService } from '../../services/API/register.service';
import { TranslateModule } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, TranslateModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerForm: FormGroup;

  constructor(private fb: FormBuilder,
    private registerService: RegisterService,
    private notificationService: PopupNotificationsService) {
    this.registerForm = fb.group({
      'username': [''],
      'password': [''],
      'passwordConfirm': ['']
    });
  }

  /**
   * register via HTTP API
   */
  protected register() {
    const val = this.registerForm.value;
    if (val.username && val.password && val.passwordConfirm) {
      if (val.password !== val.passwordConfirm) {
        this.notificationService.error("registerPasswordsDoNotMatch");
      } else {
        this.registerService.register(val.username, val.password);
      }
    } else {
      this.notificationService.error("registerErrorEnterUsernameAndPassword");
    }
  }
}
