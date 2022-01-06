import { InjectionToken } from '@angular/core';

export interface AuthCoreSettings {
  /** Base Url of identity server (eg, https://localhost:4978 or https://maydomain.com) */
  identityServerUrl: string;

  /** Should be true on production, can be false only if you are debugging app on localhost */
  refreshTokenCookieSecure: boolean;

  /** Domain name for refresh token cookies (eg, localhost or maydomain.com) */
  refreshTokenCookieDomain: string;

  /**
   * Defines which storage should be used for the access token
   * sessionStorage by default
   */
  accessTokenStorage?: 'memory' | 'localStorage' | 'sessionStorage';

  /**
   * Path to sign in api method
   * (will be combine with identityServerUrl in this way: `${this.settings.identityServerUrl}/${this.settings.signInPath})`)
   * SignIn by default
   */
  signInPath?: string;

  /**
   * Path to sign out api method
   * (will be combine with identityServerUrl in this way: `${this.settings.identityServerUrl}/${this.settings.signOutPath})`)
   * SignOut by default
   */
  signOutPath?: string;

  /**
   * Path to sign out from everywhere api method
   * (will be combine with identityServerUrl in this way: `${this.settings.identityServerUrl}/${this.settings.signOutFromEverywherePath})`)
   * SignOutFromEverywhere by default
   */
  signOutFromEverywherePath?: string;

  /**
   * Path to refresh token api method
   * (will be combine with identityServerUrl in this way: `${this.settings.identityServerUrl}/${this.settings.refreshTokenPath})`)
   * RefreshToken by default
   */
  refreshTokenPath?: string;
}

export const AUTH_CORE_SETTINGS_TOKEN = new InjectionToken<AuthCoreSettings>('AuthCoreSettings');
