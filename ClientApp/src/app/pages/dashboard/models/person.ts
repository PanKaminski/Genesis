import { Gender } from "@shared/enums/gender";
import { SelectItem } from "@shared/models/forms/select-item";
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
    images?: Picture[];
    note?: string;
    birthPlace?: Address;
    deathPlace?: Address;
    partners: SelectItem;
}