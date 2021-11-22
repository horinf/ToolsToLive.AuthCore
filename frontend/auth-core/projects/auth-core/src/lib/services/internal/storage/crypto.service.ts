import { Injectable } from '@angular/core';
// import * as CryptoJS from 'crypto-js';

@Injectable()
export class CryptoService {

  constructor() { }

  encode(plainText: string): string {
    if (!plainText) {
      return '';
    }

    return btoa(encodeURIComponent(plainText).replace(/%([0-9A-F]{2})/g,
        // eslint-disable-next-line prefer-arrow/prefer-arrow-functions
        function toSolidBytes(match, p1): string {
            return String.fromCharCode(('0x' + p1) as any);
    }));
  }

  decode(encryptedText: string | null): string | null {
    if (!encryptedText) {
      return null;
    }

    try {
      return decodeURIComponent(atob(encryptedText).split('').map((c) => {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
    } catch (error) {
      return null;
    }
  }

  // // https://www.code-sample.com/2018/12/angular-7-cryptojs-encrypt-decrypt.html

  // // The set method is used for encrypt the value.

  // encrypt(keys: string, plainText: string): string {
  //   const key = CryptoJS.enc.Utf8.parse(keys);
  //   const iv = CryptoJS.enc.Utf8.parse(keys);
  //   const encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(plainText.toString()), key,
  //   {
  //       keySize: 128 / 8,
  //       iv,
  //       mode: CryptoJS.mode.CBC,
  //       padding: CryptoJS.pad.Pkcs7
  //   });

  //   return encrypted.toString();
  // }

  // // The get method is use for decrypt the value.
  // decrypt(keys: string, encryptedText: string | null): string | null {
  //   if (!encryptedText) {
  //     return null;
  //   }

  //   try {
  //     const key = CryptoJS.enc.Utf8.parse(keys);
  //     const iv = CryptoJS.enc.Utf8.parse(keys);
  //     const decrypted = CryptoJS.AES.decrypt(encryptedText, key, {
  //         keySize: 128 / 8,
  //         iv,
  //         mode: CryptoJS.mode.CBC,
  //         padding: CryptoJS.pad.Pkcs7
  //     });

  //     return decrypted.toString(CryptoJS.enc.Utf8);
  //   } catch (error) {
  //     console.error(error);
  //     return null;
  //   }
  // }
}
