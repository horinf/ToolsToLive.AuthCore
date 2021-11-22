import { Injectable } from '@angular/core';
import { AuthData } from '../model/AuthData';

@Injectable()
export class AccessService {

  constructor() { }

  isInOneOfRoles(authData: AuthData | null, ...role: string[]): boolean {
    if (!role || !role.length) {
      return false;
    }
    const res = role.findIndex(x => this.isInRole(authData, x)) > -1;
    return res;
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
