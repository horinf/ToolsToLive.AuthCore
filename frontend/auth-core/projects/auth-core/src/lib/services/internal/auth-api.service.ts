import { HttpClient, HttpErrorResponse, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthResult } from '../../model/AuthResult';
import { AccessTokenStorage } from './storage/access-token-storage.service';
import { AuthCoreSettings, AUTH_CORE_SETTINGS_TOKEN } from '../../model/auth-core-settings';

@Injectable()
export class AuthApiService {
  private identityServerUrl: string;

  constructor(
    private http: HttpClient,
    private accessTokenStorage: AccessTokenStorage,
    @Inject(AUTH_CORE_SETTINGS_TOKEN) settings: AuthCoreSettings,
  ) {
    this.identityServerUrl = settings.identityServerUrl;
  }

  signInAsync(userNameOrEmail: string, password: string, deviceId: string): Promise<AuthResult | null> {
    const url = `${this.identityServerUrl}/SignIn`;
    const requestBody = {userNameOrEmail, password, deviceId};
    const request = this.http.post<AuthResult>(url, requestBody, { observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });

    return this.HandleResultAsync(request);
  }

  signOutAsync(): Promise<AuthResult | null> {
    const url = `${this.identityServerUrl}/SignOut`;
    const requestBody = {deviceId: ''};

    const authData = this.accessTokenStorage.load();
    let headers = new HttpHeaders();
    if (authData?.Token) {
      headers = headers.append('Authorization', `Bearer ${authData.Token.Token}`);
    }
    const request = this.http.post<AuthResult>(url, requestBody, {headers, observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });
    return this.HandleResultAsync(request);
  }

  signOutFromEverywhereAsync(): Promise<AuthResult | null> {
    const url = `${this.identityServerUrl}/SignOutFromEverywhere`;

    const authData = this.accessTokenStorage.load();
    let headers = new HttpHeaders();
    if (authData?.Token) {
      headers = headers.append('Authorization', `Bearer ${authData.Token.Token}`);
    }
    const request = this.http.post<AuthResult>(url, null, {headers, observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });
    return this.HandleResultAsync(request);
  }

  refreshTokenAsync(userId: string, refreshToken: string, deviceId: string): Promise<AuthResult | null> {
    const url = `${this.identityServerUrl}/RefreshToken`;
    const requestBody = {userId, refreshToken, deviceId};
    const request = this.http.post<AuthResult>(url, requestBody, { observe: 'response', reportProgress: false, responseType: 'json', withCredentials: true });

    return this.HandleResultAsync(request);
  }

  private HandleResultAsync(request: Observable<HttpResponse<AuthResult | null>>): Promise<AuthResult | null> {
    const promise = new Promise<AuthResult | null>((resolve, reject) => {
      request
        .subscribe((response: HttpResponse<AuthResult | null>) => {
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