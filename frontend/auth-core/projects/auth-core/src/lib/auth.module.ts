import { ModuleWithProviders, NgModule } from '@angular/core';
import { AuthService } from './services/auth.service';
import { AccessTokenStorage } from './services/internal/storage/access-token-storage.service';
import { DeviceIdService } from './services/internal/device-id.service';
import { AuthApiService } from './services/internal/auth-api.service';
import { TokenRefreshService } from './services/internal/token-refresh.service';
import { RefreshTokenStorage } from './services/internal/storage/refresh-token-storage.service';
import { SignInService } from './services/internal/sign-in.service';
import { AuthDataService } from './services/internal/auth-data.service';
import { LocalStorageService } from './services/internal/storage/local-storage.service';
import { CookieStorageService } from './services/internal/storage/cookie-storage.service';
import { CryptoService } from './services/internal/storage/crypto.service';
import { AccessService } from './services/access.service';
import { SessionStorageService } from './services/internal/storage/session-storage.service';
import { MemoryStorageService } from './services/internal/storage/memory-storage.service';
import { TokenParserService } from './services/internal/token-parser.service';

@NgModule({
  imports: [
  ]
})
export class AuthModule<TUser> {
  static forRoot<TUser>(): ModuleWithProviders<AuthModule<TUser>> {
    return {
      ngModule: AuthModule,
      providers: [
        AuthService,
        AccessService,
        TokenRefreshService,
        CryptoService,
        TokenParserService,
        LocalStorageService,
        SessionStorageService,
        MemoryStorageService,
        CookieStorageService,
        AccessTokenStorage,
        RefreshTokenStorage,
        DeviceIdService,
        AuthApiService,
        SignInService,
        AuthDataService,
      ],
    };
  }
}
