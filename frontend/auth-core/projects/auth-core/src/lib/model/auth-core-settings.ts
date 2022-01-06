import { InjectionToken } from '@angular/core';

export interface AuthCoreSettings {
  /** Base Url of identity server (eg, https://localhost:4978 or https://maydomain.com) */
  identityServerUrl: string;

  /** Should be true on production, can be false only if you are debugging app on localhost */
  refreshTokenCookieSecure: boolean;

  /** Domain name for refresh token cookies (eg, localhost or maydomain.com) */
  refreshTokenCookieDomain: string;

  accessTokenStorage: 'memory' | 'localStorage' | 'sessionStorage';
}

export const AUTH_CORE_SETTINGS_TOKEN = new InjectionToken<AuthCoreSettings>('AuthCoreSettings');
