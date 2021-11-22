import { Injectable } from '@angular/core';
import { AuthData } from '../../../model/AuthData';
import { AuthToken } from '../../../model/AuthToken';
import { AuthUserModel } from '../../../model/AuthUserModel';

@Injectable()
export class AccessTokenStorage {
  private authData: AuthData | null;

  constructor(
  ) {
    this.authData = null;
  }

  load(): AuthData | null {
    return this.authData;
  }

  save(authToken: AuthToken, user: AuthUserModel): void {
    const realDiff = authToken.ExpireDate.valueOf() - authToken.IssueDate.valueOf();
    const expDateValue = (new Date()).valueOf() + realDiff - 3000;
    authToken.BrowserExpireDate = new Date(expDateValue);
    authToken.LifeTime = realDiff;

    const authData: AuthData = { Token: authToken, User: user };
    this.authData = authData;
  }

  clean(): void {
    this.authData = null;
  }
}
