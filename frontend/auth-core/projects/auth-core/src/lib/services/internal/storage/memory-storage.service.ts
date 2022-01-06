import { Injectable } from '@angular/core';

@Injectable()
export class MemoryStorageService {
  private readonly storage: Map<string, string> = new Map();

  constructor() { }

  /** Saves value to storage (an array in the memory). */
  save(key: string, value: string): void {
    this.storage.set(key, value);
  }

  /** Loads value from storage (an array in the memory) */
  load(key: string): string | null {
    return this.storage.get(key) ?? null;
  }

  /** Deletes value in storage (an array in the memory) */
  clean(key: string): void {
    this.storage.delete(key);
  }
}
