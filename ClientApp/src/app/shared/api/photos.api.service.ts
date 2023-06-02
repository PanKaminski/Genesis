import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@environments/environment";
import { Picture } from "@shared/models/pictures/picture";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
  export class PhotosApiService {
    private readonly UPLOAD = 'api/Photos/Upload';

    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    uploadPhotos(files: File[]): Observable<Picture[]> {
        const formData = new FormData();
        for (let i = 0; i < files.length; i++) {
            formData.append('file'+i, files[i], files[i].name);
        }
        return this.http.post<Picture[]>(this.apiUrl + this.UPLOAD, formData);
    }
}