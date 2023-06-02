import { ButtonType } from "./button-type";
import { FormControl } from "./form-control";
import { FormTab } from "./form-tab";

export class Form {
    controls: FormControl[];
    tabs: FormTab[];
    buttonTypes: ButtonType[];
    isReadonly: boolean;
}