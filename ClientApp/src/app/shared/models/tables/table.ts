import { Column } from "./column";
import { Row } from "./row";

export interface Table {
    rows: Row[];
    columns: Column[];
}