import { Cell } from "./cell";

export interface Row {
    id: number;
    cells: Cell[];
    isRemovable: boolean;
    canCopy: boolean;
    isChecked?: boolean;
}