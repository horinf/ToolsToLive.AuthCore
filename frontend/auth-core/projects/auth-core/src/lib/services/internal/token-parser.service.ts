import { Injectable } from '@angular/core';
import { AccessTokenInfo } from '../../model/AccessTokenInfo';
import { ClaimModel } from '../../model/ClaimModel';
import { CryptoService } from './storage/crypto.service';

@Injectable()
export class TokenParserService {

  constructor(
    private crypto: CryptoService,
  ) { }

  parseToken(token: string): AccessTokenInfo {
    const start = token.indexOf('.') + 1;
    const end = token.indexOf('.', start);
    const payload = token.substring(start, end);
    const pl =  this.crypto.decode(payload);
    if (!pl) {
      throw new Error('Unable to parse access token');
    }
    const plObj = JSON.parse(pl);

    const claims = new Array<ClaimModel>();
    for (const key in plObj) {
      if (Object.prototype.hasOwnProperty.call(plObj, key)) {
        const element = plObj[key];
        if (key === 'exp' || key === 'nbf') {
          continue;
        }
        if (typeof element === 'string') {
          claims.push({
            Type: key,
            Value: element,
          });
        } else {
          if (typeof element === 'object' && element.length !== undefined) {
            element.forEach((claimItem: string) => {
              claims.push({
                Type: key,
                Value: claimItem,
              });
            });
          }
        }
      }
    }

    const info: AccessTokenInfo = {
      Expires: plObj.exp,
      Issued: plObj.nbf, // plObj.iat // Issued at
      Claims: claims
    };
    return info;
  }
}
