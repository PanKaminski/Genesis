import { StorageKey } from "@shared/enums/storage-key";

export function tokenGet(): string {
    return localStorage.getItem(StorageKey.JwtToken);
}