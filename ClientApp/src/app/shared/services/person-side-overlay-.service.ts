import { Injectable, OnDestroy } from "@angular/core";
import { ServerResponse } from "@shared/models/server-response";
import { Row } from "@shared/models/tables/row";
import { Observable, Subject } from "rxjs";
import { PersonEditorData } from "src/app/pages/dashboard/models/person-editor-data";
import { PersonEditorService } from "src/app/pages/dashboard/services/person-editor-service";
import { PersonSideFormComponent } from "../../pages/dashboard/components/person-side-form/person-side-form.component";
import { SideOverlayService } from "./side-overlay.service";

@Injectable({providedIn: 'root'})
  export class PersonSideOverlayService implements OnDestroy {
    private destroy$ = new Subject<void>();

    constructor(
        private readonly overlayService: SideOverlayService,
        private readonly pesonEditorService: PersonEditorService
    ) { }

    get rowUpdated$(): Observable<Row> {
        return this.pesonEditorService.rowUpdated$;
    }

    deletePersons(personIds: number[]): Observable<ServerResponse> {
        return this.pesonEditorService.deletePersons(personIds);
    }

    openPersonForm(personId: number): void {
        this.overlayService.openSidePanel<PersonSideFormComponent>(
            PersonSideFormComponent, 
            {id: personId}
        );
    }

    openNewPersonForm(params: PersonEditorData): void {
        this.overlayService.openSidePanel<PersonSideFormComponent>(
            PersonSideFormComponent, 
            params
        );
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}