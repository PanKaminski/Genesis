import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BaseApiService } from "@shared/api/base.api.service";
import { Form } from "@shared/models/forms/form";
import { Observable } from "rxjs";
import { TreeCardInfo } from "../models/tree-card-info";
import { TreeNode } from "../models/tree-node";
import { TreesList } from "../models/trees-list";

@Injectable({
    providedIn: 'any'
})
export class FamilyTreesApiService extends BaseApiService {
    private readonly TREES_LIST = 'api/FamilyTrees/GetAvailableTrees';
    private readonly TREE_PERSONS = 'api/FamilyTrees/GetTreePersons';
    private readonly CAN_LOAD_TREE = 'api/FamilyTrees/CanLoadTree';
    private readonly GET_TREE_FORM = 'api/FamilyTrees/GetTreeForm'

    constructor(http: HttpClient) {
        super(http);
     }

    loadTreeForm(treeId: number): Observable<Form> {
       return this.http.get<Form>(this.apiUrl + this.GET_TREE_FORM, { params: {treeId}});
    }
    
    getTreesList(): Observable<TreesList> {
        return this.http.get<TreesList>(this.apiUrl + this.TREES_LIST);
    }

    canLoadTree(treeId: number): Observable<boolean> {
        return this.http.get<boolean>(this.apiUrl + this.CAN_LOAD_TREE, { params: { treeId }});
    }

    getPersonsAsync(treeId: number): Observable<TreeNode[]> {
        return this.http.get<TreeNode[]>(this.apiUrl + this.TREE_PERSONS, {params: { treeId: treeId }});
    }
}