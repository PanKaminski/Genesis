import { Injectable, OnDestroy } from "@angular/core";
import { PagedModel } from "@shared/models/paged-model";
import { ServerDataResponse } from "@shared/models/server-response";
import { Observable, Subject } from "rxjs";
import { ContactsApiService } from "../api/contacts.api.service";
import { ChangeConnectionStatusModel } from "../models/change-connection-status-model";
import { UpdateConnectionStatusResponse } from "../models/update-card-status-change";
import { ConnectionCard } from "../models/user-card";
import { UsersToolCode } from "../models/users-tool-code";

@Injectable({
    providedIn: 'any'
})
export class ContactsService implements OnDestroy {
    protected readonly destroy$ = new Subject<void>();


    constructor(
        private readonly api: ContactsApiService,
        ) {
    }

    getUsers(toolCode: UsersToolCode, page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        switch (toolCode) {
            case UsersToolCode.Connections: return this.getConnections(page, pageSize);
            case UsersToolCode.Invites: return this.getInvites(page, pageSize);
            case UsersToolCode.Pendings: return this.getPendings(page, pageSize);
            default: return this.searchUsers(page, pageSize);
        }
    }

    changeConnectionStatus(
            model: ChangeConnectionStatusModel
        ): Observable<ServerDataResponse<UpdateConnectionStatusResponse>> {
        return this.api.changeConnectionStatus(model);
    }

    private getConnections(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.api.getConnections(page, pageSize);
    }

    private getInvites(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.api.getInvites(page, pageSize);
    }

    private getPendings(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.api.getPendings(page, pageSize);
    }

    private searchUsers(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.api.searchUsers(page, pageSize);
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}