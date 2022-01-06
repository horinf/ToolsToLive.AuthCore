import { ActivatedRouteSnapshot, CanActivate, CanLoad, Data, Route, Router, RouterStateSnapshot, UrlSegment, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AuthData } from '../model/AuthData';
import { AccessService } from '../services/access.service';
import { isPlatformBrowser } from '@angular/common';
import { AuthUserModel } from '../model/AuthUserModel';

/**
 * Without any parameters guards checks if user is authenticated
 * With route.data['roles'] guard chcks if users is authenticated and has at least one of the roles
 */
@Injectable({
  providedIn: 'root'
})
export class AuthCoreGuard<TUser extends AuthUserModel> implements CanLoad, CanActivate {

  constructor(private authService: AuthService<TUser>,
              private accessService: AccessService<TUser>,
              @Inject(PLATFORM_ID) private platformId: any,
              private router: Router) {
  }
  canLoad(
    route: Route,
    segments: UrlSegment[]): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
      return new Promise((resolve, reject) => {
        this.authService.getAuthDataAsync()
          .then((data: AuthData<TUser> | null) => {
            if (data === null) {
              if (isPlatformBrowser(this.platformId)) {
                this.router.navigate(['/']);
              }
              resolve(false);
            }
            if (!this.checkRoles(route.data, data)) {
              if (isPlatformBrowser(this.platformId)) {
                this.router.navigate(['/']);
              }
              resolve(false);
            }

            resolve(true);
          })
          .catch(err => {
            reject(err);
          });
      });
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return new Promise((resolve, reject) => {
      this.authService.getAuthDataAsync()
        .then((data: AuthData<TUser> | null) => {
          if (data === null) {
            if (isPlatformBrowser(this.platformId)) {
              this.router.navigate(['/']);
            }
            resolve(false);
          }
          if (!this.checkRoles(route.data, data)) {
            if (isPlatformBrowser(this.platformId)) {
              this.router.navigate(['/']);
            }
            resolve(false);
          }

          resolve(true);
        })
        .catch(err => {
          reject(err);
        });
    });
  }

  checkRoles(data: Data | undefined, authData: AuthData<TUser> | null): boolean {
    if (!data) {
      return true;
    }
    const roles = data.roles as Array<string>;

    if (roles && roles.length > 0) {
      return this.accessService.isInOneOfRoles(authData, ...roles);
    }
    return true;
  }
}
