import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "@environments/environment";
import { Form } from "@shared/models/forms/form";
import { FormImage } from "@shared/models/forms/form-file";
import { ServerDataResponse, ServerResponse } from "@shared/models/server-response";
import { Observable } from "rxjs";
import { PersonEditorData } from "../models/person-editor-data";
import { PersonSaveFormData } from "../models/person-save-from-data";
import { TreeNode } from "../models/tree-node";

@Injectable({
    providedIn: 'any'
})
export class PersonEditorApiService {
    private readonly GET_FORM = 'api/Persons/GetForm';
    private readonly SAVE_FORM = 'api/Persons/SaveForm';
    private readonly DELETE_USER = 'api/Persons/DeletePerson';

    private readonly apiUrl = environment.apiUrl;

    constructor(private http: HttpClient) { }

    getForm(data: PersonEditorData): Observable<Form> {
        return this.http.post<Form>(this.apiUrl + this.GET_FORM, data);
    }

    saveForm(data: PersonSaveFormData): Observable<ServerDataResponse<TreeNode>> {
        return this.http.post<ServerDataResponse<TreeNode>>(this.apiUrl + this.SAVE_FORM, data);
    }

    deletePerson(personId: number): Observable<ServerResponse> {
        return this.http.delete<ServerResponse>(this.apiUrl + this.DELETE_USER, { params: { personId }});
    }
}