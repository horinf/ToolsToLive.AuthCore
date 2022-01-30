import { Injectable } from '@angular/core';
import { AuthData } from '../model/AuthData';
import { ClaimModel } from '../model/ClaimModel';
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

  async getRolesAsync(): Promise<string[]> {
    const authData = await this.authService.getAuthDataAsync();
    return this.getRoles(authData);
  }

  getRoles(authData: AuthData<TUser> | null): string[] {
    const roles = new Array<string>();
    if (!authData || !authData.Claims || authData.Claims.length === 0) {
      return roles;
    }
    const claims = this.getClaims(authData, AccessService.RoleClaim);
    claims.forEach(claim => {
      roles.push(claim.Value);
    });
    return roles;
  }

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

  async getClaimsAsync(type: string): Promise<ClaimModel[]> {
    const authData = await this.authService.getAuthDataAsync();
    return this.getClaims(authData, type);
  }

  getClaims(authData: AuthData<TUser> | null, type: string): ClaimModel[] {
    if (!authData?.Claims || !(authData?.Claims.length > 0)) {
      return new Array<ClaimModel>();
    }

    return authData.Claims.filter(x => x.Type === type);
  }

  async hasAnyClaimAsync(type: string): Promise<boolean> {
    const authData = await this.authService.getAuthDataAsync();
    return this.hasAnyClaim(authData, type);
  }

  hasAnyClaim(authData: AuthData<TUser> | null, type: string): boolean {
    if (!authData?.Claims || !(authData?.Claims.length > 0)) {
      return false;
    }
    if (authData.Claims.findIndex(x => x.Type === type) > -1) {
      return true;
    }
    return false;
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
