import { Row } from "@shared/models/tables/row";
import { TreeNode } from "./tree-node";

export interface PersonSaveResponse {
    node: TreeNode;
    row: Row;
}