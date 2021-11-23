import { Injectable } from '@angular/core';
import { AuthData } from '../model/AuthData';
import { AuthService } from './auth.service';

@Injectable()
export class AccessService {

  constructor(
    private authService: AuthService
  ) { }

  async isInOneOfRolesAsync(...role: string[]): Promise<boolean> {
    const authData = await this.authService.getAuthDataAsync();
    return this.isInOneOfRoles(authData, ...role);
  }

  isInOneOfRoles(authData: AuthData | null, ...roles: string[]): boolean {
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

  isInRole(authData: AuthData | null, role: string): boolean {
    if (!authData?.User?.Roles) {
      return false;
    }
    if (authData.User.Roles.findIndex(x => x.Id === role) > -1) {
      return true;
    }
    return false;
  }
}
