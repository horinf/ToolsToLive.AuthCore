import { Inject, Injectable } from '@angular/core';
import { AuthCoreSettings, AUTH_CORE_SETTINGS_TOKEN } from '../../../model/auth-core-settings';
import { RefreshToken } from '../../../model/RefreshToken';
import { CookieStorageService } from './cookie-storage.service';

@Injectable()
export class RefreshTokenStorage {
  private readonly storageName: string = 'mydomain_rt';

  constructor(
    @Inject(AUTH_CORE_SETTINGS_TOKEN) settings: AuthCoreSettings,
    private cookieStorage: CookieStorageService,
  ) {
    this.storageName = settings.refreshTokenCookieDomain + '_rt';
  }

  load(): RefreshToken | null {
    const fromStore: string | null = this.cookieStorage.load(this.storageName);
    if (!fromStore) {
      return null;
    }

    try {
      const result: RefreshToken = JSON.parse(fromStore);

      result.IssueDate = new Date(result.IssueDate);
      result.ExpireDate = new Date(result.ExpireDate);
      result.BrowserExpireDate = new Date(result.BrowserExpireDate);
      return result;

    } catch (error) {
      console.error(error);
      return null;
    }
  }

  isExist(): boolean {
    return this.cookieStorage.isExist(this.storageName);
  }

  save(authToken: RefreshToken): void {
    const realDiff = authToken.ExpireDate.valueOf() - authToken.IssueDate.valueOf();
    const expDateValue = (new Date()).valueOf() + realDiff - 3000;
    authToken.BrowserExpireDate = new Date(expDateValue); // time in the browser may differ from the time on the server
    authToken.LifeTime = realDiff;
    const toStore = JSON.stringify(authToken);
    this.cookieStorage.save(this.storageName, toStore, authToken.BrowserExpireDate);
  }

  clean(): void {
    this.cookieStorage.clean(this.storageName);
  }
}
