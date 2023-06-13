import { HttpClient } from "@angular/common/http";
import { Injectable, OnDestroy } from "@angular/core";
import { BaseApiService } from "@shared/api/base.api.service";
import { Table } from "@shared/models/tables/table";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'any'
  })
  export class DataManagerApiService extends BaseApiService {    
    private readonly GET_PERSONS_TABLE = 'api/DataManager/GetPersonsTable'

    constructor(http: HttpClient) {
        super(http);
    }

    loadPersonsTable(): Observable<Table> {
       return this.http.get<Table>(this.apiUrl + this.GET_PERSONS_TABLE);
    }
  }