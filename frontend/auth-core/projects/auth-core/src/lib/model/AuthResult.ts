import { AuthResultType } from './AuthResultType';
import { RefreshToken } from './RefreshToken';

export interface AuthResult<TUser> {
  Result: AuthResultType;
  User: TUser;
  AccessToken: string;
  RefreshToken: RefreshToken;
  IsSuccess: boolean;
}
