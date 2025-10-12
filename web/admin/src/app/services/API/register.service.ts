import { Injectable } from '@angular/core';
import { ApiServiceBase } from './api-base.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class RegisterService extends ApiServiceBase {

  constructor(private router: Router) {
    super("Accounts");
  }

  public async register(username: string, password: string) {
    const result = await this.apiPost<boolean>("", {
      email: username,
      password: password,
      confirmPassword: password
    });
    if (result) {
      this.notificationService.info("registerSuccessful");
      this.router.navigate(['/login']);
    }
  }
}
