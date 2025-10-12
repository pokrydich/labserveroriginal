import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { IUser } from '../models/entity/user';
import { UserApiService } from './API/user-api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly cStorageKey = 'apiAuthToken';

  private authToken: string | null = null;
  private loggedInUser: IUser | null = null;

  constructor(private router: Router,
    private userService: UserApiService) {}
  /**
   * return session JWT token
   * @returns JWT token for requests authorization
   */
  public getAuthToken(): string | null {
    if (this.authToken == null) {
      this.authToken = localStorage.getItem(this.cStorageKey);
    }
    return this.authToken;
  }
  /**
   * Login with JWT token (from HTTP login API)
   * @param token JWT token received from server
   */
  public login(token: string) {
    this.authToken = token;
    localStorage.setItem(this.cStorageKey, token);
    this.router.navigate(['/']);
  }
  /**
   * Logout from account
   */
  public logout() {
    this.authToken = null;
    localStorage.removeItem(this.cStorageKey);
    this.loggedInUser = null;
    this.router.navigate(['/login']);
  }

  public async getLoggedInUser(): Promise<IUser> {
    if (this.loggedInUser === null) {
      this.loggedInUser = await this.userService.getAuthStatus();
      console.log(`[*] Retrieved user information. Roles: ${this.loggedInUser.roles}`);
    }
    return this.loggedInUser;
  }

  public getLoggedInUserSynced(): IUser | null {
    return this.loggedInUser;
  }

  public getUserRoles(): string[] {
    return this.loggedInUser
              ? this.loggedInUser.roles
              : [];
  }
}
