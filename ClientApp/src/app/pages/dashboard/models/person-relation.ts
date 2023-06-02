import { PersonRelationType } from "./person-relation-type";

export interface PersonRelation {
    from: number;
    to: number;
    type: PersonRelationType;
}