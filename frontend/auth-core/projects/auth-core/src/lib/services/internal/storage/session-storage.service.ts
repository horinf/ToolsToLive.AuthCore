import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CryptoService } from './crypto.service';

@Injectable()
export class SessionStorageService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private crypto: CryptoService,
    // private cookieStorage: CookieStorageService, // can be used as fallback - if sessionStorage is not supported, then fallback to cookies
  ) { }

  /** Saves value to storage. Works in the 'browser side' only, when rendering on the server side - does nothing */
  save(key: string, value: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (sessionStorage) {
        try {
          const encryptedValue = this.crypto.encode(value);
          sessionStorage.setItem(key, encryptedValue);
        } catch (error) {
          // in some browsers (like old Safari) sessionStorage is defined, but if you try set or read value - it throws an error
          throw new Error('Unable to save data to sessionStorage');
        }
      } else {
        throw new Error('Unable to save data to sessionStorage');
      }
    }
  }

  /** Loads value from storage. Works in the 'browser side' only, when rendering on the server side - returns null */
  load(key: string): string | null {
    let fromStore = null;
    if (isPlatformBrowser(this.platformId)) {
      if (sessionStorage){
        try {
          const encryptedValue = sessionStorage.getItem(key);
          fromStore = this.crypto.decode(encryptedValue);
        } catch (error) {
          // in some browsers sessionStorage is defined, but if you try set or read value - it throws an error
          throw new Error('Unable to load data from sessionStorage');
        }
      } else {
          throw new Error('Unable to load data from sessionStorage');
      }
    }
    return fromStore;
  }

  /** Deletes value in storage. Works in the 'browser side' only, when rendering on the server side - does nothing */
  clean(key: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (sessionStorage) {
        // in some browsers sessionStorage is defined, but if you try set or read value - it throws an error
        try {
          sessionStorage.removeItem(key);
        } catch (error) {
          throw new Error('Unable to remove data from sessionStorage');
        }
      } else {
          throw new Error('Unable to remove data from sessionStorage');
      }
    }
  }
}
