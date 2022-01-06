import { Injectable } from '@angular/core';
import { AuthData } from '../../model/AuthData';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthApiService } from './auth-api.service';
import { DeviceIdService } from './device-id.service';
import { RefreshTokenStorage } from './storage/refresh-token-storage.service';
import { AuthResultType } from '../../model/AuthResultType';
import { AuthUserModel } from '../../model/AuthUserModel';
import { TokenParserService } from './token-parser.service';

@Injectable()
export class SignInService<TUser extends AuthUserModel> {

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
        if (data?.IsSuccess) {
          const info = this.tokenParser.parseToken(data.Token.Token);

          this.accessTokenStorage.save(data.Token, data.User);
          this.refreshTokenStorage.save(data.RefreshToken);


          const authData = {User: data.User, Token : data.Token};
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
            Token: null,
            User: null,
          };
          reject(authres);
      });
    });
    return result;
  }

  decode(encryptedText: string | null): string | null {
    if (!encryptedText) {
      return null;
    }

    try {
      return decodeURIComponent(atob(encryptedText).split('').map((c) => {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
    } catch (error) {
      return null;
    }
  }

  customsignIn(authResult: AuthResult<TUser>): AuthData<TUser> | null {
    if (authResult?.IsSuccess) {
      this.accessTokenStorage.save(authResult.Token, authResult.User);
      this.refreshTokenStorage.save(authResult.RefreshToken);
      return {User: authResult.User, Token : authResult.Token};
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
