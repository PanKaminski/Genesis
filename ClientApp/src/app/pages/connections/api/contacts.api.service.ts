import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseApiService } from "@shared/api/base.api.service";
import { PagedModel } from "@shared/models/paged-model";
import { ServerDataResponse } from "@shared/models/server-response";
import { Observable } from "rxjs";
import { ChangeConnectionStatusModel } from "../models/change-connection-status-model";
import { UpdateConnectionStatusResponse } from "../models/update-card-status-change";
import { ConnectionCard } from "../models/user-card";

@Injectable({
    providedIn: 'any'
})
export class ContactsApiService extends BaseApiService {

    private readonly CONNECTIONS = 'api/Connections/GetContacts';
    private readonly INVITES = 'api/Connections/GetInvites';
    private readonly PENDINGS = 'api/Connections/GetPendings';
    private readonly SEARCH = 'api/Connections/SearchUsers';
    private readonly UPDATE_STATUS = 'api/Connections/UpdateConnectionStatus';

    constructor(http: HttpClient) {
        super(http);
    }

    getConnections(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.http.get<PagedModel<ConnectionCard>>(this.apiUrl + this.CONNECTIONS, {params: { page, pageSize }});
    }

    changeConnectionStatus(
        model: ChangeConnectionStatusModel
    ): Observable<ServerDataResponse<UpdateConnectionStatusResponse>> {
        return this.http.post<ServerDataResponse<UpdateConnectionStatusResponse>>(
            this.apiUrl + this.UPDATE_STATUS, 
            model
        );
    }
    getInvites(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.http.get<PagedModel<ConnectionCard>>(this.apiUrl + this.INVITES, {params: { page, pageSize }});
    }

    getPendings(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.http.get<PagedModel<ConnectionCard>>(this.apiUrl + this.PENDINGS, {params: { page, pageSize }});
    }

    searchUsers(page: number, pageSize: number): Observable<PagedModel<ConnectionCard>> {
        return this.http.get<PagedModel<ConnectionCard>>(this.apiUrl + this.SEARCH, {params: { page, pageSize }});
    }
}