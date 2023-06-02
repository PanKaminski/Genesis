import { Injectable, OnDestroy } from "@angular/core";
import { PhotosApiService } from "@shared/api/photos.api.service";
import { Picture } from "@shared/models/pictures/picture";
import { Observable } from "rxjs";
import { Subject } from "rxjs";

@Injectable({providedIn: 'root'})
  export class PhotosService implements OnDestroy {
    private destroy$ = new Subject<void>();

    constructor(private api: PhotosApiService) { }

    uploadPhotos(photos: File[]): Observable<Picture[]> {
        return this.api.uploadPhotos(photos);
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}