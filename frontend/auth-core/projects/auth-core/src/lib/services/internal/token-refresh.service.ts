import { Injectable } from '@angular/core';
import { AuthData } from '../../model/AuthData';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthApiService } from './auth-api.service';
import { RefreshTokenStorage } from './storage/refresh-token-storage.service';

@Injectable()
export class TokenRefreshService {

  private resultPromise?: Promise<AuthData>;

  constructor(
    private authApiService: AuthApiService,
    private accessTokenStorage: AccessTokenStorage,
    private refreshTokenStorage: RefreshTokenStorage,
    ) { }

  refreshTokenAsync(): Promise<AuthData> {
    // Call the logic and api only once, and if someone else wants to get data -- return promise
    if (this.resultPromise) {
      return this.resultPromise;
    }

    const resultPromise = new Promise<AuthData>((resolve, reject) => {
      const refreshToken = this.refreshTokenStorage.load();
      if (!refreshToken){
        reject();
        return;
      }

      const authDataRequest = this.authApiService.refreshTokenAsync(refreshToken.UserId, refreshToken.Token, '');
      authDataRequest.then((data: AuthResult | null) => {
        if (data?.IsSuccess) {
          this.accessTokenStorage.save(data.Token, data.User);
          this.refreshTokenStorage.save(data.RefreshToken);

          const authData = {User: data.User, Token : data.Token};
          resolve(authData);
        } else {
          this.refreshTokenStorage.clean();
          this.accessTokenStorage.clean();
          console.warn('Something went wrong - unable to refresh token');
          reject('Something went wrong - unable to refresh token');
        }
      }).catch((error: any) => {
        console.error(error);
        reject(error);
      }).finally(() => {
      });
    });

    this.resultPromise = resultPromise;
    resultPromise.finally(() => {
      this.resultPromise = undefined;
    });

    return this.resultPromise;
  }
}
