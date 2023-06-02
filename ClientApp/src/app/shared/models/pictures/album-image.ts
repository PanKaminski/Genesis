import { SafeUrl } from '@angular/platform-browser';

export interface AlbumImage {
    id: number;
    src: string | SafeUrl;
    thumb: string;
    caption?: string;
}