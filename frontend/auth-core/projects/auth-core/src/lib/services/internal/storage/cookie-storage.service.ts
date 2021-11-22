import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { CryptoService } from './crypto.service';
import { AuthCoreSettings, AUTH_CORE_SETTINGS_TOKEN } from '../../../model/auth-core-settings';

export interface CookieDict {
  [key: string]: string;
}

export interface CookieOptions {
  path?: string;
  domain?: string;
  expires?: string | Date;
  secure?: boolean;
  httpOnly?: boolean;
  sameSite?: boolean | 'lax' | 'strict' | 'none';
  storeUnencoded?: boolean;
}

@Injectable()
export class CookieStorageService {
  constructor(
    @Inject(PLATFORM_ID) private platformId: any,
    private crypto: CryptoService,
    @Inject(AUTH_CORE_SETTINGS_TOKEN) private settings: AuthCoreSettings,
    @Inject(DOCUMENT) private document: any
  ) {
  }

  save(key: string, value: string, expires?: Date): void {
    if (isPlatformBrowser(this.platformId)) {
      const now = new Date();
      if (!expires) {
        expires = new Date(now.valueOf() + 86400000); // 86400000 ms = 1 day
      }

      const encryptedValue = this.crypto.encode(value);
      const secure = this.settings.refreshTokenCookieSecure;
      const domain = this.settings.refreshTokenCookieDomain;
      this.put(key, encryptedValue, {expires, httpOnly: false, path: '/', sameSite: 'strict', secure, domain});
    }
  }

  isExist(key: string): boolean {
    if (isPlatformBrowser(this.platformId)) {
      const value = this.get(key);
      return this.isPresent(value);
    }
    return false;
  }

  load(key: string): string | null {
    let fromStore = null;
    if (isPlatformBrowser(this.platformId)) {
      const encryptedValue = this.get(key);
      fromStore = this.crypto.decode(encryptedValue);
    }
    return fromStore;
  }

  clean(key: string): void {
    if (isPlatformBrowser(this.platformId)) {
      const now = new Date();
      const expires = new Date(now.valueOf() - 60000);
      const secure = this.settings.refreshTokenCookieSecure;
      const domain = this.settings.refreshTokenCookieDomain;

      this.put(key, undefined, {expires, httpOnly: false, path: '/', sameSite: 'strict', secure, domain});
    }
  }

  private put(key: string, value: string | undefined, options: CookieOptions): void {
    this.document.cookie = this.buildCookieString(key, value, options);
  }

  private get(key: string): string {
    const cookieString = this.document.cookie || '';
    const cookieDict = this.parseCookieString(cookieString);
    return cookieDict?.[key];
  }


  private parseCookieString(currentCookieString: string): CookieDict {
    let lastCookies: CookieDict = {};
    let lastCookieString = '';
    let cookieArray: string[];
    let cookie: string;
    let i: number;
    let index: number;
    let name: string;
    if (currentCookieString !== lastCookieString) {
      lastCookieString = currentCookieString;
      cookieArray = lastCookieString.split('; ');
      lastCookies = {};
      for (i = 0; i < cookieArray.length; i++) {
        cookie = cookieArray[i];
        index = cookie.indexOf('=');
        if (index > 0) {  // ignore nameless cookies
          name = this.safeDecodeURIComponent(cookie.substring(0, index));
          // the first value that is seen for a cookie is the most
          // specific one.  values for the same cookie name that
          // follow are for less specific paths.
          if (this.isNil((lastCookies)[name])) {
            lastCookies[name] = this.safeDecodeURIComponent(cookie.substring(index + 1));
          }
        }
      }
    }
    return lastCookies;
  }

  private safeDecodeURIComponent(str: string): string {
    try {
      return decodeURIComponent(str);
    } catch (e) {
      return str;
    }
  }

  private isNil(obj: any): boolean {
    return obj === undefined || obj === null;
  }

  private isPresent(obj: any): boolean {
    return !this.isNil(obj);
  }

  private isString(obj: any): obj is string {
    return typeof obj === 'string';
  }

  private buildCookieString(name: string, value: string | undefined, options?: CookieOptions): string {
    let expires: string | Date | undefined = options?.expires;
    let val: string;
    if (this.isNil(value)) {
      expires = 'Thu, 01 Jan 1970 00:00:00 GMT';
      val = '';
    } else {
      val = value as string;
    }
    if (this.isString(expires)) {
      expires = new Date(expires);
    }
    const cookieValue = options?.storeUnencoded ? value : encodeURIComponent(val);
    let str = encodeURIComponent(name) + '=' + cookieValue;
    str += options?.path ? ';path=' + options.path : '';
    str += options?.domain ? ';domain=' + options.domain : '';
    str += expires ? ';expires=' + expires.toUTCString() : '';
    str += options?.sameSite ? '; SameSite=' + options.sameSite : '';
    str += options?.secure ? ';secure' : '';
    str += options?.httpOnly ? '; HttpOnly' : '';

    // per http://www.ietf.org/rfc/rfc2109.txt browser must allow at minimum:
    // - 300 cookies
    // - 20 cookies per unique domain
    // - 4096 bytes per cookie
    const cookieLength = str.length + 1;
    if (cookieLength > 4096) {
      console.log('Cookie \'' + name + '\' possibly not set or overflowed because it was too large (' + cookieLength + ' > 4096 bytes)!');
    }
    return str;
  }
}

