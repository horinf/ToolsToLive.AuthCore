import { AuthResultType } from './AuthResultType';
import { RefreshToken } from './RefreshToken';

export interface AuthResult<TUser> {
  Result: AuthResultType;
  User: TUser | null;
  AccessToken: string | null;
  RefreshToken: RefreshToken | null;
  IsSuccess: boolean;
}
