import { Gender } from "@shared/enums/gender";
import { PersonRelationType } from "./person-relation-type";

export interface PersonEditorData {
    id?: number;
    gender?: Gender;
    newRelation?: PersonRelationType;
    personRelationFrom?: number;
    personRelationTo?: number;
}