import { AuthToken } from './AuthToken';
import { AuthUserModel } from './AuthUserModel';

export interface AuthData {
    User: AuthUserModel;
    Token: AuthToken;
}
