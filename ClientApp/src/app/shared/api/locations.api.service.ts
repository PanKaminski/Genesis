import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@environments/environment";
import { SelectItem } from "@shared/models/forms/select-item";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
  export class LocationsApiService {
    private readonly ALL_COUNTRIES = 'api/Locations/GetCountriesList';
    private readonly CITIES = 'api/Locations/GetCitiesList';

    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getCountriesItemsList(): Observable<SelectItem[]> {
        return this.http.get<SelectItem[]>(this.apiUrl + this.ALL_COUNTRIES);
    }

    getCitiesItemsList(countryCode: string): Observable<SelectItem[]> {
        return this.http.get<SelectItem[]>(this.apiUrl + this.CITIES, { params: {countryCode} });
    }
}