import { Injectable, OnDestroy } from "@angular/core";
import { Table } from "@shared/models/tables/table";
import { Observable, Subject } from "rxjs";
import { DataManagerApiService } from "../api/data-mamanger.api.service";

@Injectable({
    providedIn: 'any'
  })
  export class DataManagerService implements OnDestroy {
    private destroy$ = new Subject<void>();

    constructor(private readonly api: DataManagerApiService) {

    }

    loadPersonsTable(): Observable<Table> {
        return this.api.loadPersonsTable();
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
  }