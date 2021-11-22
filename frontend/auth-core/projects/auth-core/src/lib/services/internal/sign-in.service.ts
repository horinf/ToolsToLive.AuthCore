import { Injectable } from '@angular/core';
import { AuthData } from '../../model/AuthData';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthApiService } from './auth-api.service';
import { DeviceIdService } from './device-id.service';
import { RefreshTokenStorage } from './storage/refresh-token-storage.service';
import { AuthResultType } from '../../model/AuthResultType';

@Injectable()
export class SignInService {

constructor(
  private deviceIdService: DeviceIdService,
  private authApiService: AuthApiService,
  private accessTokenStorage: AccessTokenStorage,
  private refreshTokenStorage: RefreshTokenStorage,
) { }

signInAsync(userName: string, password: string): Promise<AuthData> {
  const result = new Promise<AuthData>((resolve, reject) => {
    const deviceId = this.deviceIdService.createDeviceId();

    const authDataRequest = this.authApiService.signInAsync(userName, password, deviceId);
    authDataRequest.then((data: AuthResult | null) => {
      if (data?.IsSuccess) {
        this.accessTokenStorage.save(data.Token, data.User);
        this.refreshTokenStorage.save(data.RefreshToken);

        const authData = {User: data.User, Token : data.Token};
        resolve(authData);
      } else {
        reject(this.getFaultMessage(data));
      }

    }).catch((error: any) => {
        console.error(error);
        reject('Something went wrong - unable to sign in');
    });
  });
  return result;
}

customsignInAsync(authResult: AuthResult): AuthData | null {
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

private getFaultMessage(data: AuthResult | null): string {
  if (!data) {
    return 'Something went wrong - server did not return the data';
  }

  switch (data.Result) {
    case AuthResultType.Fault:
      return 'Неудачная попытка входа';
      case AuthResultType.LockedOut:
      return 'User is locked, please try again later';
      case AuthResultType.PasswordIsWrong:
      return 'Password is wrong';
      case AuthResultType.RefreshTokenWrong:
      return 'Refresh token is wrong';
      case AuthResultType.UserNotFound:
      return 'User not found';
    default:
      return 'Something went wrong';
  }
}
}