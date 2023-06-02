import { HttpClient } from "@angular/common/http";
import { Injectable, OnDestroy } from "@angular/core";
import { environment } from "@environments/environment";
import { Subject } from "rxjs";

@Injectable({
    providedIn: 'any'
})
export class BaseApiService implements OnDestroy {
    protected readonly destroy$ = new Subject<void>();
    protected readonly apiUrl = environment.apiUrl;

    constructor(protected readonly http: HttpClient) { }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}