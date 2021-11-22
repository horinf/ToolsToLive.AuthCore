import { AuthUserRoleModel } from './AuthUserRoleModel';

export interface AuthUserModel {
  Id: string;
  UserName: string;
  Email: string;
  FirstName: string;
  MiddleName: string;
  LastName: string;
  Roles: AuthUserRoleModel[];
}
