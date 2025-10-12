import { Injectable } from '@angular/core';
import { ApiServiceBase } from './api-base.service';
import { IUser } from '../../models/entity/user';

@Injectable({
  providedIn: 'root'
})
export class UserApiService extends ApiServiceBase {
  public readonly cSupportedRoles: { [id: string]: number } = {
    "Administrator": 0,
    "Professor": 1,
    "Assistant": 2
  };

  public getAllRoles(): string[] {
    return Object.keys(this.cSupportedRoles);
  }

  constructor() {
    super("rest/Users");
  }

  public async getAuthStatus(): Promise<IUser> {
    return await this.apiGet<IUser>("/authstatus");
  }

  public async getUsers(): Promise<IUser[]> {
    return await this.apiGet<IUser[]>("");
  }

  public async getUserById(id: number): Promise<IUser> {
    return await this.apiGet<IUser>(`/${id}`);
  }

  public async addUserRole(userId: number, roleId: number) {
    await this.apiGet<any>(`/${userId}/roles?role=${roleId}`);
  }

  public async deleteUserRole(userId: number, roleId: number) {
    await this.apiDelete<any>(`/${userId}/roles?role=${roleId}`);
  }
}
