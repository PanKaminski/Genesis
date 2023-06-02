import { ComponentRef, Injectable } from "@angular/core";
import { Form } from "@shared/models/forms/form";
import { ResultCode, ServerDataResponse, ServerResponse } from "@shared/models/server-response";
import { NotificationService } from "@shared/services/notification.service";
import { map, Observable, Subject } from "rxjs";
import { PersonEditorApiService } from "../api/person-editor.api.service";
import { PersonEditorData } from "../models/person-editor-data";
import { PersonSaveFormData } from "../models/person-save-from-data";
import { TreeNode } from "../models/tree-node";

@Injectable({
    providedIn: 'root'
})
export class PersonEditorService {
    component: ComponentRef<any>;

    private readonly treeNodeUpdatedSub$ = new Subject<TreeNode>();
    treeNodeUpdated$ = this.treeNodeUpdatedSub$.asObservable();

    private readonly treeNodeDeletedSub$ = new Subject<number>();
    treeNodeDeleted$ = this.treeNodeDeletedSub$.asObservable();

    constructor(
        private readonly api: PersonEditorApiService,
        private readonly notifier: NotificationService,
    ) { }

    savePerson(data: PersonSaveFormData): Observable<ServerDataResponse<TreeNode>> {
        return this.api.saveForm(data)
            .pipe(
                map((result: ServerDataResponse<TreeNode>) => {
                    if (result.code === ResultCode.Failed)
                        this.notifier.notifyErrorMessage(result.message);
                    else {
                        this.notifier.notifySuccess(result.message);
                        this.treeNodeUpdatedSub$.next(result.data);
                    }
                    return result;
                }),
            );
    }

    loadPersonForm(data: PersonEditorData): Observable<Form> {
        return this.api.getForm(data);
    }

    deletePerson(personId: number): Observable<ServerResponse> {
        return this.api.deletePerson(personId).pipe(
            map((result: ServerResponse) => {
                if (result.code === ResultCode.Failed)
                    this.notifier.notifyErrorMessage(result.message);
                else {
                    this.notifier.notifySuccess(result.message);
                    this.treeNodeDeletedSub$.next(personId);
                }
                return result;
            }),
        );
    }
}