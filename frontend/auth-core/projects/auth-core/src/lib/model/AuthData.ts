import { AuthToken } from './AuthToken';
import { AuthUserModel } from './AuthUserModel';

export interface AuthData<TUser extends AuthUserModel> {
    User: TUser;
    Token: AuthToken;
}
