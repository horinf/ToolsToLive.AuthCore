import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { AuthData } from '..//model/AuthData';
import { AuthResult } from '../model/AuthResult';
import { AuthUserModel } from '../model/AuthUserModel';
import { AuthDataService } from './internal/auth-data.service';
import { DeviceIdService } from './internal/device-id.service';
import { SignInService } from './internal/sign-in.service';
import { TokenRefreshService } from './internal/token-refresh.service';

@Injectable()
export class AuthService<TUser extends AuthUserModel> {
  public userChangingTrack = new Subject<AuthData<TUser> | null>();

  private currentAuthData: AuthData<TUser> | null;

  constructor(
    private deviceIdService: DeviceIdService,
    private signInService: SignInService<TUser>,
    private authDataService: AuthDataService<TUser>,
    private tokenRefreshService: TokenRefreshService<TUser>,
    ) {
    this.currentAuthData = null;
  }

  getAuthDataAsync(): Promise<AuthData<TUser> | null> {
    const authDataPromise = this.authDataService.getAuthDataAsync();
    authDataPromise.then((authData: AuthData<TUser> | null) => {
      if (!this.compareAuthData(this.currentAuthData, authData)) {
        this.authUpdate(authData);
      }
    }).catch(err => {});
    return authDataPromise;
  }

  signInAsync(userName: string, password: string): Promise<AuthData<TUser> | null> {
    const signInResult = this.signInService.signInAsync(userName, password);
    signInResult.then((authData: AuthData<TUser>) => {
      this.authUpdate(authData);
    }).catch(() => {
      this.authUpdate(null);
    });
    return signInResult;
  }

  // Manual token refresh -- can be useful when user's claims should be changed or smth like this
  async refreshTokenAsync(): Promise<AuthData<TUser>> {
    const authData = await this.tokenRefreshService.refreshTokenAsync();
    this.authUpdate(authData);
    return authData;
  }

  customsignInAsync(authResult: AuthResult<TUser>): AuthData<TUser> | null {
    const result = this.signInService.customsignInAsync(authResult);
    this.authUpdate(result);
    return result;
  }

  signOutAsync(): Promise<void> {
    const signInResult = this.signInService.signOutAsync();
    signInResult.then(() => {
      this.authUpdate(null);
    });
    return signInResult;
  }

  createDeviceId(): string {
    return this.deviceIdService.createDeviceId();
  }

  private authUpdate(authData: AuthData<TUser> | null): void {
    this.currentAuthData = authData;
    this.userChangingTrack.next(authData);
  }

  private compareAuthData(a1: AuthData<TUser> | null, a2: AuthData<TUser> | null): boolean {
    if (a1 === null && a2 === null) {
      return true;
    }

    if (a1 === null || a2 === null) {
      return false;
    }

    if (a1.Token.UserId === a2.Token.UserId &&
        a1.Token.Token === a2.Token.Token &&
        a1.User.Id === a2.User.Id &&
        a1.User.Roles === a2.User.Roles &&
        a1.User.Roles.length === a2.User.Roles.length) {
        return true;
      }

    return false;
  }
}
