import { InjectionToken } from '@angular/core';

export interface AuthCoreSettings {
  /** Base Url of identity server (eg, https://localhost:4978 or https://maydomain.com) */
  identityServerUrl: string;
  refreshTokenCookieSecure: boolean;
  refreshTokenCookieDomain: string;
}

export const AUTH_CORE_SETTINGS_TOKEN = new InjectionToken<AuthCoreSettings>('AuthCoreSettings');
