import { Injectable } from '@angular/core';
import { AuthData } from '../../model/AuthData';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthApiService } from './auth-api.service';
import { RefreshTokenStorage } from './storage/refresh-token-storage.service';
import { AuthResultType } from '../../model/AuthResultType';

@Injectable()
export class TokenRefreshService<TUser> {

  private resultPromise?: Promise<AuthData<TUser>>;

  constructor(
    private authApiService: AuthApiService<TUser>,
    private accessTokenStorage: AccessTokenStorage<TUser>,
    private refreshTokenStorage: RefreshTokenStorage,
    ) { }

  refreshTokenAsync(): Promise<AuthData<TUser>> {
    // Call the logic and api only once, and if someone else wants to get data -- return promise
    if (this.resultPromise) {
      return this.resultPromise;
    }

    const resultPromise = new Promise<AuthData<TUser>>((resolve, reject) => {
      const refreshToken = this.refreshTokenStorage.load();
      if (!refreshToken){
        reject();
        return;
      }

      const authDataRequest = this.authApiService.refreshTokenAsync(refreshToken.UserId, refreshToken.Token, '');
      authDataRequest.then((data: AuthResult<TUser> | null) => {
        if (data?.IsSuccess) {
          const authData = this.accessTokenStorage.save(data);
          this.refreshTokenStorage.save(data.RefreshToken);
          resolve(authData);
        } else {
          this.refreshTokenStorage.clean();
          this.accessTokenStorage.clean();
          console.warn('Something went wrong - unable to refresh token');
          reject(data?.Result ?? AuthResultType.Failed);
        }
      }).catch((error: any) => {
        console.error(error);
        reject(AuthResultType.Failed);
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
