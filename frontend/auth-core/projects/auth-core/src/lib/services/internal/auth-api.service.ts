import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthCoreSettings, AUTH_CORE_SETTINGS_TOKEN } from '../../model/auth-core-settings';
import { AuthUserModel } from '../../model/AuthUserModel';

@Injectable()
export class AuthApiService<TUser extends AuthUserModel> {

  constructor(
    private http: HttpClient,
    private accessTokenStorage: AccessTokenStorage<TUser>,
    @Inject(AUTH_CORE_SETTINGS_TOKEN) private settings: AuthCoreSettings,
  ) {
  }

  signInAsync(userNameOrEmail: string, password: string, deviceId: string): Promise<AuthResult<TUser> | null> {
    const path = this.settings.signInPath ?? 'SignIn';
    const url = `${this.settings.identityServerUrl}/${path}`;
    const requestBody = {userNameOrEmail, password, deviceId};
    const request = this.http.post<AuthResult<TUser>>(url, requestBody, { observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });

    return this.HandleResultAsync(request);
  }

  signOutAsync(): Promise<AuthResult<TUser> | null> {
    const path = this.settings.signOutPath ?? 'SignOut';
    const url = `${this.settings.identityServerUrl}/${path}`;
    const requestBody = {deviceId: ''};

    const authData = this.accessTokenStorage.load();
    let headers = new HttpHeaders();
    if (authData?.Token) {
      headers = headers.append('Authorization', `Bearer ${authData.Token.Token}`);
    }
    const request = this.http.post<AuthResult<TUser>>(url, requestBody, {headers, observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });
    return this.HandleResultAsync(request);
  }

  signOutFromEverywhereAsync(): Promise<AuthResult<TUser> | null> {
    const path = this.settings.signOutFromEverywherePath ?? 'SignOutFromEverywhere';
    const url = `${this.settings.identityServerUrl}/${path}`;

    const authData = this.accessTokenStorage.load();
    let headers = new HttpHeaders();
    if (authData?.Token) {
      headers = headers.append('Authorization', `Bearer ${authData.Token.Token}`);
    }
    const request = this.http.post<AuthResult<TUser>>(url, null, {headers, observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });
    return this.HandleResultAsync(request);
  }

  refreshTokenAsync(userId: string, refreshToken: string, deviceId: string): Promise<AuthResult<TUser> | null> {
    const path = this.settings.refreshTokenPath ?? 'RefreshToken';
    const url = `${this.settings.identityServerUrl}/${path}`;
    const requestBody = {userId, refreshToken, deviceId};
    const request = this.http.post<AuthResult<TUser>>(url, requestBody, { observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });

    return this.HandleResultAsync(request);
  }

  private HandleResultAsync(request: Observable<HttpResponse<AuthResult<TUser> | null>>): Promise<AuthResult<TUser> | null> {
    const promise = new Promise<AuthResult<TUser> | null>((resolve, reject) => {
      request
        .subscribe((response: HttpResponse<AuthResult<TUser> | null>) => {
          const data = response.body;
          if (data) {
            if (data.Token?.ExpireDate) {
              data.Token.ExpireDate = new Date(data.Token.ExpireDate);
            }
            if (data.Token?.IssueDate) {
              data.Token.IssueDate = new Date(data.Token.IssueDate);
            }
            if (data.RefreshToken?.ExpireDate) {
              data.RefreshToken.ExpireDate = new Date(data.RefreshToken.ExpireDate);
            }
            if (data.RefreshToken?.IssueDate) {
              data.RefreshToken.IssueDate = new Date(data.RefreshToken.IssueDate);
            }
          }
          resolve(data);
        }, (error: HttpErrorResponse) => {
          reject(error);
        }, () => {
          resolve(null);
        });
    });

    return promise;
  }
}
