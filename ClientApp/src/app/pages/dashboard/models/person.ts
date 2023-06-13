import { Gender } from "@shared/enums/gender";
import { Picture } from "@shared/models/pictures/picture";
import { Address } from "./address";

export interface Person {
    id: number;
    firstName: string;
    middleName?: string;
    lastName?: string;
    dateOfBirth?: Date;
    dateOfDeath?: Date;
    gender: Gender;
    avatar?: Picture;
    note?: string;
    birthPlace?: Address;
    deathPlace?: Address;
    partnersIds: number[];
    childrenIda: number[];
    motherId?: number;
    fatherId?: number;
}