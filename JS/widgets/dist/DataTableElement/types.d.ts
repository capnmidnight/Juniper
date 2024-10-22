import { FieldName } from "@juniper-lib/util";
export type ColumnType = "array" | "csv" | "boolean" | "button" | "color" | "date" | "dollars" | "enum" | "html" | "integer" | "link" | "none" | "number" | "percent" | "string";
export interface BaseColumnDef<T> {
    field: FieldName<T>;
    header: string;
    type: ColumnType;
    hidden?: boolean;
}
export interface SheetDef<T> {
    name: string;
    columns: BaseColumnDef<T>[];
    data: any[][];
}
//# sourceMappingURL=types.d.ts.map