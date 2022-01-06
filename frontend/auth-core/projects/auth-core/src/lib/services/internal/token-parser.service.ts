import { Injectable } from '@angular/core';
import { TokenInfo } from '../../model/AuthToken';
import { ClaimModel } from '../../model/ClaimModel';
import { CryptoService } from './storage/crypto.service';

@Injectable()
export class TokenParserService {

  constructor(
    private crypto: CryptoService,
  ) { }

  parseToken(token: string): TokenInfo {
    const start = token.indexOf('.') + 1;
    const end = token.indexOf('.', start);
    const payload = token.substring(start, end);
    const pl =  this.crypto.decode(payload);
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
            element.forEach(claimItem => {
              claims.push({
                Type: key,
                Value: claimItem,
              });
            });
          }
        }
      }
    }

    const info: TokenInfo = {
      ExpireDate: new Date(plObj.exp * 1000),
      IssueDate: new Date(plObj.nbf * 1000),
      Claims: claims
    };
    return info;
  }
}
