import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { AuthCoreSettings, AUTH_CORE_SETTINGS_TOKEN } from '../../../model/auth-core-settings';
import { AuthData } from '../../../model/AuthData';
import { AuthToken } from '../../../model/AuthToken';
import { AuthUserModel } from '../../../model/AuthUserModel';
import { LocalStorageService } from './local-storage.service';
import { MemoryStorageService } from './memory-storage.service';
import { SessionStorageService } from './session-storage.service';

@Injectable()
export class AccessTokenStorage<TUser extends AuthUserModel>  {
  private readonly storageKey: string = 'mydomain_at';

  constructor(
    @Inject(AUTH_CORE_SETTINGS_TOKEN) private settings: AuthCoreSettings,
    @Inject(PLATFORM_ID) private platformId: any,
    private sessionStorage: SessionStorageService,
    private localStorage: LocalStorageService,
    private memoryStorage: MemoryStorageService,
  ) {
    this.storageKey = this.settings.refreshTokenCookieDomain + '_at';
  }

  load(): AuthData<TUser> | null {
    let authString: string | null;

    if (isPlatformBrowser(this.platformId)) {
      switch (this.settings.accessTokenStorage ?? 'sessionStorage') {
        case 'memory':
          authString = this.memoryStorage.load(this.storageKey);
          break;
        case 'sessionStorage':
          try {
            authString = this.sessionStorage.load(this.storageKey);
          } catch (error) {
            console.warn('Unable to use sessionStorage, fallback to memory storage');
            authString = this.memoryStorage.load(this.storageKey); // fallback to memory storage
          }
          break;
        case 'localStorage':
          try {
            authString = this.localStorage.load(this.storageKey);
          } catch (error) {
            console.warn('Unable to use localStorage, fallback to memory storage');
            authString = this.memoryStorage.load(this.storageKey); // fallback to memory storage
          }
          break;
        default:
          throw new Error('Unexpected value of accessTokenStorage parameter');
      }
    } else {
      authString = this.memoryStorage.load(this.storageKey); // on the server side memory storage can be used only
    }

    try {
      return JSON.parse(authString);
    } catch (error) {
      console.warn('Unable to parse token from storage. Storage will be cleared.');
      this.clean();
      return null;
    }
  }

  save(authToken: AuthToken, user: TUser): void {
    const realDiff = authToken.ExpireDate.valueOf() - authToken.IssueDate.valueOf();
    const expDateValue = (new Date()).valueOf() + realDiff - 3000;
    authToken.BrowserExpireDate = new Date(expDateValue);
    authToken.LifeTime = realDiff;

    const authData: AuthData<TUser> = { Token: authToken, User: user };
    const authString = JSON.stringify(authData);
    if (isPlatformBrowser(this.platformId)) {
      switch (this.settings.accessTokenStorage ?? 'sessionStorage') {
        case 'memory':
          this.memoryStorage.save(this.storageKey, authString);
          break;
        case 'sessionStorage':
          try {
            this.sessionStorage.save(this.storageKey, authString);
          } catch (error) {
            console.warn('Unable to use sessionStorage, fallback to memory storage');
            this.memoryStorage.save(this.storageKey, authString); // fallback to memory storage
          }
          break;
        case 'localStorage':
          try {
            this.localStorage.save(this.storageKey, authString);
          } catch (error) {
            console.warn('Unable to use localStorage, fallback to memory storage');
            this.memoryStorage.save(this.storageKey, authString); // fallback to memory storage
          }
          break;
        default:
          throw new Error('Unexpected value of accessTokenStorage parameter');
      }
    } else {
      this.memoryStorage.save(this.storageKey, authString); // on the server side memory storage can be used only
    }
  }

  clean(): void {
    if (isPlatformBrowser(this.platformId)) {
      switch (this.settings.accessTokenStorage ?? 'sessionStorage') {
        case 'memory':
          this.memoryStorage.clean(this.storageKey);
          break;
        case 'sessionStorage':
          try {
            this.sessionStorage.clean(this.storageKey);
          } catch (error) {
            console.warn('Unable to use sessionStorage, fallback to memory storage');
            this.memoryStorage.clean(this.storageKey); // fallback to memory storage
          }
          break;
        case 'localStorage':
          try {
            this.localStorage.clean(this.storageKey);
          } catch (error) {
            console.warn('Unable to use localStorage, fallback to memory storage');
            this.memoryStorage.clean(this.storageKey); // fallback to memory storage
          }
          break;
        default:
          throw new Error('Unexpected value of accessTokenStorage parameter');
      }
    } else {
      this.memoryStorage.clean(this.storageKey); // on the server side memory storage can be used only
    }
  }
}
