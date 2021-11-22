import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AuthData } from '../model/AuthData';
import { AccessService } from '../services/access.service';
import { isPlatformBrowser } from '@angular/common';

/**
 * Without any parameters guards checks if user is authenticated
 * With route.data['roles'] guard chcks if users is authenticated and has at least one of the roles
 */
@Injectable({
  providedIn: 'root'
})
export class AuthCoreGuard implements CanActivate {

  constructor(private authService: AuthService,
              private accessService: AccessService,
              @Inject(PLATFORM_ID) private platformId: any,
              private router: Router) {
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return new Promise((resolve, reject) => {
      this.authService.getAuthDataAsync()
        .then((data: AuthData | null) => {
          if (data === null) {
            if (isPlatformBrowser(this.platformId)) {
              this.router.navigate(['/']);
            }
            resolve(false);
          }
          if (!this.checkRoles(route, data)) {
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

  checkRoles(route: ActivatedRouteSnapshot, authData: AuthData | null): boolean {
    const roles = route.data['roles'] as Array<string>;

    if (roles && roles.length > 0) {
      return this.accessService.isInOneOfRoles(authData, ...roles);
    }
    return true;
  }
}
