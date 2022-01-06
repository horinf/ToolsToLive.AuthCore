import { ClaimModel } from './ClaimModel';

export interface AuthUserModel {
  Id: string;
  UserName: string;
  Email: string;
  Roles?: string[];
  Claims?: ClaimModel;
}
