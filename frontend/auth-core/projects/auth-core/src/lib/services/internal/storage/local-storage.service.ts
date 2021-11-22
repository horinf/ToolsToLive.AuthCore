import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CookieStorageService } from './cookie-storage.service';
import { CryptoService } from './crypto.service';

@Injectable()
export class LocalStorageService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private crypto: CryptoService,
    private cookieStorage: CookieStorageService,
  ) { }

  save(key: string, value: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (localStorage) {
        const encryptedValue = this.crypto.encode(value);
        localStorage.setItem(key, encryptedValue);
      } else {
        this.cookieStorage.save(key, value);
      }
    }
  }

  load(key: string): string | null {
    let fromStore = null;
    if (isPlatformBrowser(this.platformId)) {
      if (localStorage){
        const encryptedValue = localStorage.getItem(key);
        fromStore = this.crypto.decode(encryptedValue);
      } else {
        fromStore = this.cookieStorage.load(key);
      }
    }
    return fromStore;
  }

  clean(key: string): void {
    if (isPlatformBrowser(this.platformId)) {
      if (localStorage) {
        localStorage.removeItem(key);
      } else {
        this.cookieStorage.clean(key);
      }
    }
  }
}
