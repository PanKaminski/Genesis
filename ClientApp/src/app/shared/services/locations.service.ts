import { Injectable, OnDestroy } from "@angular/core";
import { LocationsApiService } from "@shared/api/locations.api.service";
import { SelectItem } from "@shared/models/forms/select-item";
import { Observable } from "rxjs";
import { Subject } from "rxjs";

@Injectable({providedIn: 'root'})
  export class LocationsService implements OnDestroy {
    private destroy$ = new Subject<void>();

    constructor(private api: LocationsApiService) { }

    getCountriesItemsList(): Observable<SelectItem[]>{
        return this.api.getCountriesItemsList();
    }

    getCitiesItemsList(countryCode: string): Observable<SelectItem[]>{
        return this.api.getCitiesItemsList(countryCode);
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}