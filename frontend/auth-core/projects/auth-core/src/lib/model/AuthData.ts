import { ClaimModel } from './ClaimModel';

export interface AuthData<TUser> {
    User: TUser;
    AccessToken: string;

    BrowserExpireDate: number; // Date.valueOf()
    LifeTime: number;
    Claims: ClaimModel[];
}
