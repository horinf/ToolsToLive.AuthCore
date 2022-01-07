import { Injectable } from '@angular/core';
import { AuthData } from '../model/AuthData';
import { AuthService } from './auth.service';

@Injectable()
export class AccessService<TUser> {

  public static readonly TokenTransportClaim = 'AuthCoreTokenTransport';
  public static readonly RoleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
  public static readonly UserIdClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
  public static readonly UserNameClaim = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

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
    if (!authData?.Claims) {
      return false;
    }
    return this.hasClaim(authData, AccessService.RoleClaim, role);
  }

  async hasClaimAsync(type: string, value: string): Promise<boolean> {
    const authData = await this.authService.getAuthDataAsync();
    return this.hasClaim(authData, type, value);
  }

  hasClaim(authData: AuthData<TUser> | null, type: string, value: string): boolean {
    if (!authData?.Claims || !(authData?.Claims.length > 0)) {
      return false;
    }
    if (authData.Claims.findIndex(x => x.Type === type && x.Value === value) > -1) {
      return true;
    }
    return false;
  }
}
