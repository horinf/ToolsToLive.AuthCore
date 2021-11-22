import { Injectable } from '@angular/core';

@Injectable()
export class DeviceIdService {

constructor(
) { }

createDeviceId(): string {
  const id = this.generateString(32);
  return id;
}

private generateString(length: number): string {
  const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  let result = '';
  const charactersLength = characters.length;
  for ( let i = 0; i < length; i++ ) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
  }

  return result;
}
}
