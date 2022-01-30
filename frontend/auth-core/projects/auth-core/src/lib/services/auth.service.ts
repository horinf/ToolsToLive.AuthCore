import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { AuthData } from '..//model/AuthData';
import { AuthResult } from '../model/AuthResult';
import { AuthDataService } from './internal/auth-data.service';
import { DeviceIdService } from './internal/device-id.service';
import { SignInService } from './internal/sign-in.service';
import { TokenRefreshService } from './internal/token-refresh.service';

@Injectable()
export class AuthService<TUser> {
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

  /**
   * SignIn
   * @param userName  - user name (login)
   * @param password - password (plain text)
   * @returns 'AuthData<TUser>' if successed (resolved) or 'AuthResult<TUser> | null' if not (rejected)
   */
  signInAsync(userName: string, password: string): Promise<AuthData<TUser> | null> {
    const signInResult = this.signInService.signInAsync(userName, password);
    signInResult.then((authData: AuthData<TUser>) => {
      this.authUpdate(authData);
    }).catch((err: AuthResult<TUser> | null) => {
      this.authUpdate(null);
    });
    return signInResult;
  }

  /**
   * Manual token refresh -- can be useful when user's claims should be changed or smth like this
   * @returns AuthData<TUser>
   */
  async refreshTokenAsync(): Promise<AuthData<TUser>> {
    const authData = await this.tokenRefreshService.refreshTokenAsync();
    this.authUpdate(authData);
    return authData;
  }

  customsignIn(authResult: AuthResult<TUser>): AuthData<TUser> | null {
    const result = this.signInService.customsignIn(authResult);
    this.authUpdate(result);
    return result;
  }

  signOutAsync(): Promise<void> {
    const signInResult = this.signInService.signOutAsync();
    signInResult.then(() => {
      this.authUpdate(null);
    }).catch(() => {
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

  /**
   * If equal - returns true
   */
  private compareAuthData(a1: AuthData<TUser> | null, a2: AuthData<TUser> | null): boolean {
    if (a1 === null && a2 === null) {
      return true;
    }

    if (a1 === null || a2 === null) {
      return false;
    }

    if (a1.Claims?.length !== a2.Claims?.length) {
      return false;
    }

    if (a1.Claims && a1.Claims.length > 0 && a2.Claims && a2.Claims.length > 0) {
      let isDifferent = false;
      for (let index = 0; index < a1.Claims.length; index++) {
        if (a1.Claims[index] !== a2.Claims[index]) {
          isDifferent = true;
        }
      }
      if (isDifferent) {
        return false;
      }
    }

    // if (a1.Token.UserId === a2.Token.UserId &&
    //     a1.Token.Token === a2.Token.Token &&
    //     a1.User.Id === a2.User.Id &&
    //     a1.User.Roles === a2.User.Roles &&
    //     a1.User.Roles.length === a2.User.Roles.length) {
    //     return true;
    //   }

    return true;
  }
}
