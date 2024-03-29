import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CryptoService } from './crypto.service';

@Injectable()
export class LocalStorageService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private crypto: CryptoService,
    // private cookieStorage: CookieStorageService, // can be used as fallback - if localStorage is not supported, then fallback to cookies
  ) { }

  /** Saves value to storage. Works in the 'browser side' only, when rendering on the server side - does nothing */
  save(key: string, value: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (localStorage) {
        try {
          const encryptedValue = this.crypto.encode(value);
          localStorage.setItem(key, encryptedValue);
        } catch (error) {
          // in some browsers (like old Safari) localStorage is defined, but if you try set or read value - it throws an error
          throw new Error('Unable to save data to localStorage');
        }
      } else {
        throw new Error('Unable to save data to localStorage');
      }
    }
  }

  /** Loads value from storage. Works in the 'browser side' only, when rendering on the server side - returns null */
  load(key: string): string | null {
    let fromStore = null;
    if (isPlatformBrowser(this.platformId)) {
      if (localStorage){
        try {
          const encryptedValue = localStorage.getItem(key);
          fromStore = this.crypto.decode(encryptedValue);
        } catch (error) {
          // in some browsers localStorage is defined, but if you try set or read value - it throws an error
          throw new Error('Unable to load data from localStorage');
        }
      } else {
          throw new Error('Unable to load data from localStorage');
      }
    }
    return fromStore;
  }

  /** Deletes value in storage. Works in the 'browser side' only, when rendering on the server side - does nothing */
  clean(key: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (localStorage) {
        // in some browsers localStorage is defined, but if you try set or read value - it throws an error
        try {
          localStorage.removeItem(key);
        } catch (error) {
          throw new Error('Unable to remove data from localStorage');
        }
      } else {
          throw new Error('Unable to remove data from localStorage');
      }
    }
  }
}
