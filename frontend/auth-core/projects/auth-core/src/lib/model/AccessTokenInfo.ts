import { ClaimModel } from './ClaimModel';

export interface AccessTokenInfo {
  /**
   * Issue date (seconds past 1970-01-01 00:00:00Z)
   */
  Issued: number; // set by server (according to date and time on the server)

  /**
   * Expiration date (seconds past 1970-01-01 00:00:00Z)
   */
  Expires: number; // set by server (according to date and time on the server)

  /**
   * All other claims
   */
  Claims: ClaimModel[];
}
