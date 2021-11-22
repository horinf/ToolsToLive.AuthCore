import { isPlatformServer } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { AuthData } from '../../model/AuthData';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { RefreshTokenStorage } from './storage/refresh-token-storage.service';
import { TokenRefreshService } from './token-refresh.service';

@Injectable()
export class AuthDataService {

  private resultPromise?: Promise<AuthData | null>;

  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private accessTokenStorage: AccessTokenStorage,
    private tokenRefreshService: TokenRefreshService,
    private refreshTokenStorage: RefreshTokenStorage,
    ) { }

  getAuthDataAsync(): Promise<AuthData | null> {
    // Call the logic and api only once, and if someone else wants to get data -- return promise
    if (this.resultPromise) {
      return this.resultPromise;
    }

    const resultPromise = new Promise<AuthData | null>((resolve, reject) => {
      // On the server side user is never authenticated!
      if (isPlatformServer(this.platformId)) {
        resolve(null);
      }

      const authData = this.accessTokenStorage.load();
      if (this.refreshTokenStorage.isExist() === false) {
        this.accessTokenStorage.clean();
        resolve(null);
        return;
      }

      if (!authData) {
        this.tokenRefreshService.refreshTokenAsync()
          .then((data: AuthData) => {
            resolve(data);
          }).catch((error: any) => {
            resolve(null);
          }).finally(() => {
          });
        return;
      }

      // const realDiff = authData.Token.ExpireDate.valueOf() - authData.Token.IssueDate.valueOf();
      // const timePass = ((new Date()).valueOf() - authData.Token.IssuedBrouserDate.valueOf()) + 5000;
      const now = (new Date()).valueOf();
      const exp = authData.Token.BrowserExpireDate.valueOf();

      // Если срок жизни токена истёк
      if (now > exp) {
        this.tokenRefreshService.refreshTokenAsync()
          .then((data: AuthData) => {
            resolve(data);
          }).catch((error: any) => {
            resolve(null);
          }).finally(() => {
          });
        return;
      } else {
        // Refresh token in the background if half of the token lifetim has passed
        if ((exp - now) < authData.Token.LifeTime / 2) {
          this.tokenRefreshService.refreshTokenAsync();
        }
      }

      resolve(authData);
    });

    this.resultPromise = resultPromise;
    resultPromise.finally(() => {
      this.resultPromise = undefined;
    });

    return resultPromise;
  }
}
