import { Injectable } from '@angular/core';
import { AuthData } from '../model/AuthData';
import { AuthUserModel } from '../model/AuthUserModel';
import { AuthService } from './auth.service';

@Injectable()
export class AccessService<TUser extends AuthUserModel> {

  constructor(
    private authService: AuthService<TUser>
  ) { }

  async isInOneOfRolesAsync(...role: string[]): Promise<boolean> {
    const authData = await this.authService.getAuthDataAsync();
    return this.isInOneOfRoles(authData, ...role);
  }

  isInOneOfRoles(authData: AuthData<TUser> | null, ...roles: string[]): boolean {
    if (!roles || !roles.length) {
      return false;
    }
    const res = roles.findIndex(x => this.isInRole(authData, x)) > -1;
    return res;
  }

  async isInRoleAsync(role: string): Promise<boolean> {
    const authData = await this.authService.getAuthDataAsync();
    return this.isInRole(authData, role);
  }

  isInRole(authData: AuthData<TUser> | null, role: string): boolean {
    if (!authData?.User?.Roles) {
      return false;
    }
    if (authData.User.Roles.findIndex(x => x === role) > -1) {
      return true;
    }
    return false;
  }
}
