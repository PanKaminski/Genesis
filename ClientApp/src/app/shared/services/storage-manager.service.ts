import { Injectable } from "@angular/core";
import { StorageKey } from "@shared/enums/storage-key";

@Injectable({providedIn: 'root'})
export class StorageManager {
    saveItem(key: StorageKey, value: unknown): void {
        localStorage.removeItem(key);
        localStorage.setItem(key, JSON.stringify(value));
    }

    saveJwt(token: string): void {
        localStorage.setItem(StorageKey.JwtToken, token);
    }

    getItem(key: StorageKey): unknown {
        const value = localStorage.getItem(key);
        try {
            return JSON.parse(value);
        } catch (e) {
            if (value === 'undefined') {
              return void 0;
            }

            return value;
        }
    }

    removeItem(key: StorageKey): void {
        localStorage.removeItem(key);
    }
}