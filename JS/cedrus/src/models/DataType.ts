import { isNullOrUndefined, isObject, isString } from "@juniper-lib/util";

export type DataType =
    | "Unknown"
    | "Boolean"
    | "BooleanArray"
    | "Integer"
    | "IntegerArray"
    | "Decimal"
    | "DecimalArray"
    | "String"
    | "StringArray"
    | "Date"
    | "DateArray"
    | "Link"
    | "File";

type DataTypes =
    | unknown
    | boolean
    | boolean[]
    | number
    | number[]
    | number
    | number[]
    | string
    | string[]
    | Date
    | Date[];

export interface DataTypeMap extends Record<DataType, DataTypes> {
    Unknown: unknown;
    Boolean: boolean;
    BooleanArray: boolean[];
    Integer: number;
    IntegerArray: number[];
    Decimal: number;
    DecimalArray: number[];
    String: string;
    StringArray: string[];
    Date: Date;
    DateArray: Date[];
    Link: string;
    File: string;
}

export function hasDataType(obj: unknown, dataType?: DataType) {
    return isObject(obj)
        && "dataType" in obj
        && isString(obj.dataType)
        && (isNullOrUndefined(dataType)
            || obj.dataType === dataType);
}