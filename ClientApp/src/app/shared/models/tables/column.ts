import { ColumnType } from "./column-type";

export interface Column {
    id: number;
    name: string;
    columnType: ColumnType;
    entityType?: number;
}