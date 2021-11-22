export interface AuthToken {
  UserId: string;
  Token: string;
  IssueDate: Date;
  ExpireDate: Date;

  // Date of token expiration in current browser (just in case if browser time is different from server)
  BrowserExpireDate: Date;
  LifeTime: number;
}
