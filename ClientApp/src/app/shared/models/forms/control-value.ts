export interface ControlValue {
    entityType: number;
    value: FormValueType;
}

export type FormValueType = number | number[] | string | string[] | Date;
