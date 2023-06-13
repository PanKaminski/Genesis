import { ControlValue } from "@shared/models/forms/control-value";
import { Picture } from "@shared/models/pictures/picture";

export interface TreeFormData {
    treeId: number;
    values: ControlValue[];
    picture?: Picture;
}