import { ButtonType } from "./button-type";

export interface Button {
    type: ButtonType;
    name: string;
    isDisabled: boolean;
}