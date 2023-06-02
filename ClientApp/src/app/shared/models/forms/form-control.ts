import { ControlType } from "./control-type";
import { SelectItem } from "./select-item";

export class FormControl {
    name: string;
    entityType: number;
    type: ControlType;
    value: any;
    items?: SelectItem[];
    isReadonly: boolean;
    isMulty: boolean;
    isRequired: boolean;
    tabId: number;
}