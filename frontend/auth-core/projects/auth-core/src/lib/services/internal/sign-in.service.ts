import { Injectable } from '@angular/core';
import { AuthData } from '../../model/AuthData';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthApiService } from './auth-api.service';
import { DeviceIdService } from './device-id.service';
import { RefreshTokenStorage } from './storage/refresh-token-storage.service';
import { AuthResultType } from '../../model/AuthResultType';
import { TokenParserService } from './token-parser.service';

@Injectable()
export class SignInService<TUser> {

constructor(
  private deviceIdService: DeviceIdService,
  private authApiService: AuthApiService<TUser>,
  private accessTokenStorage: AccessTokenStorage<TUser>,
  private refreshTokenStorage: RefreshTokenStorage,
  private tokenParser: TokenParserService,
) { }

/**
 * sign in
 * @param userName - user name (login)
 * @param password - password
 * @returns 'AuthData<TUser>' if successed or 'AuthResult<TUser> | null' if not
 */
  signInAsync(userName: string, password: string): Promise<AuthData<TUser>> {
    const result = new Promise<AuthData<TUser>>((resolve, reject) => {
      const deviceId = this.deviceIdService.createDeviceId();

      const authDataRequest = this.authApiService.signInAsync(userName, password, deviceId);
      authDataRequest.then((data: AuthResult<TUser> | null) => {
        if (data?.IsSuccess && data.RefreshToken && data.AccessToken && data.User) {
          const authData = this.accessTokenStorage.save(data);
          this.refreshTokenStorage.save(data.RefreshToken);
          resolve(authData);
        } else {
          reject(data);
        }
      }).catch((error: any) => {
          console.error(error);
          const authres: AuthResult<TUser> = {
            IsSuccess: false,
            Result: AuthResultType.Failed,
            RefreshToken: null,
            AccessToken: null,
            User: null,
          };
          reject(authres);
      });
    });
    return result;
  }

  customsignIn(authResult: AuthResult<TUser>): AuthData<TUser> | null {
    if (authResult?.IsSuccess && authResult.RefreshToken && authResult.AccessToken && authResult.User) {
      const authData = this.accessTokenStorage.save(authResult);
      this.refreshTokenStorage.save(authResult.RefreshToken);
      return authData;
    }
    return null;
  }

  signOutAsync(): Promise<void> {
    const result = new Promise<void>((resolve, reject) => {
      this.authApiService.signOutAsync()
        .then(() => {
        }).catch((err) => {
          console.log(err);
          console.warn('Something went wrong - unable to sign out');
        }).finally(() => {
          this.accessTokenStorage.clean();
          this.refreshTokenStorage.clean();
          resolve();
        });
    });
    return result;
  }

  signOutFromEverywhereAsync(): Promise<void> {
    const result = new Promise<void>((resolve, reject) => {
      this.authApiService.signOutFromEverywhereAsync()
        .then(() => {
        }).catch((err) => {
          console.log(err);
          console.warn('Something went wrong - unable to sign out');
        }).finally(() => {
          this.accessTokenStorage.clean();
          this.refreshTokenStorage.clean();
          resolve();
        });
    });
    return result;
  }
}
