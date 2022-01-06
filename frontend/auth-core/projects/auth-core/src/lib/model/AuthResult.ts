import { AuthResultType } from './AuthResultType';
import { AuthToken } from './AuthToken';
import { AuthUserModel } from './AuthUserModel';

export interface AuthResult<TUser extends AuthUserModel> {
  Result: AuthResultType;
  User: TUser;
  Token: AuthToken;
  RefreshToken: AuthToken;
  IsSuccess: boolean;
}
