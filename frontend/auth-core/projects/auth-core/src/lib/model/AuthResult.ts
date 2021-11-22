import { AuthResultType } from './AuthResultType';
import { AuthToken } from './AuthToken';
import { AuthUserModel } from './AuthUserModel';

export interface AuthResult {
  Result: AuthResultType;
  User: AuthUserModel;
  Token: AuthToken;
  RefreshToken: AuthToken;
  IsSuccess: boolean;
}
