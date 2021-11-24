import { Inject, Injectable } from '@angular/core';
import { AuthCoreSettings, AUTH_CORE_SETTINGS_TOKEN } from '../../../model/auth-core-settings';
import { AuthToken } from '../../../model/AuthToken';
import { CookieStorageService } from './cookie-storage.service';

@Injectable()
export class RefreshTokenStorage {
  private storageName = 'mydomain_rt';

  constructor(
    @Inject(AUTH_CORE_SETTINGS_TOKEN) settings: AuthCoreSettings,
    private cookieStorage: CookieStorageService,
  ) {
    this.storageName = settings.refreshTokenCookieDomain + '_rt';
  }

  load(): AuthToken | null {
    const fromStore: string | null = this.cookieStorage.load(this.storageName);
    if (!fromStore) {
      return null;
    }

    try {
      const result = JSON.parse(fromStore);

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

  save(authToken: AuthToken): void {
    const realDiff = authToken.ExpireDate.valueOf() - authToken.IssueDate.valueOf();
    const expDateValue = (new Date()).valueOf() + realDiff - 3000;
    authToken.BrowserExpireDate = new Date(expDateValue);
    authToken.LifeTime = realDiff;
    const toStore = JSON.stringify(authToken);
    this.cookieStorage.save(this.storageName, toStore, authToken.BrowserExpireDate);
  }

  clean(): void {
    this.cookieStorage.clean(this.storageName);
  }
}
