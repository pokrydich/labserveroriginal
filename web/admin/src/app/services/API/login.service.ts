import { Injectable } from '@angular/core';
import { ApiServiceBase } from './api-base.service';
import { AuthService } from '../auth.service';

@Injectable({
  providedIn: 'root'
})
export class LoginService extends ApiServiceBase {

  constructor(private authService: AuthService) {
    super("Login");
  }

  public async login(email: string, password: string) {
    const token = await this.apiPost<string>("", {
      email: email,
      password: password,
      rememberMe: true
    });
    if (token !== null) {
      this.authService.login(token);
    }
  }
}
