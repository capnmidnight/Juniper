/**
 * Make sure this stays in sync with the DataType enum in ../../../NETCore/Cedrus/Entities/DataType.cs
 */

import { isNullOrUndefined, isObject, isString } from "@juniper-lib/util";
import { FileViewValue } from "@juniper-lib/widgets";

export type StorageType =
    | "Single"
    | "Array"
    | "TimeSeries";

export const StorageTypeValues: StorageType[] = [
    "Single",
    "Array",
    "TimeSeries"
];

const storageTypes = new Set(StorageTypeValues);
export function isStorageType(value: string): value is StorageType {
    return storageTypes.has(value as StorageType);
}

export type DataType =
    | "Unknown"
    | "Boolean"
    | "Integer"
    | "Decimal"
    | "Currency"
    | "String"
    | "Enumeration"
    | "LongText"
    | "Date"
    | "Link"
    | "File";

export const DataTypeValues: DataType[] = [
    "Unknown",
    "Boolean",
    "Integer",
    "Decimal",
    "Currency",
    "String",
    "Enumeration",
    "LongText",
    "Date",
    "Link",
    "File"
];


const dataTypes = new Set(DataTypeValues);
export function isDataType(value: string): value is DataType {
    return dataTypes.has(value as DataType);
}

export function hasDataType(obj: unknown, dataType?: DataType) {
    return isObject(obj)
        && "type" in obj
        && isString(obj.type)
        && (isNullOrUndefined(dataType)
            || obj.type === dataType);
}

export function hasStorageType(obj: unknown, storageType?: StorageType) {
    return isObject(obj)
        && "storage" in obj
        && isString(obj.storage)
        && (isNullOrUndefined(storageType)
            || obj.storage === storageType);
}

export type TimeSeries<T> = {
    date: Date;
    value: T;
}

export type NumberTimeSeries = TimeSeries<number>;
export type StringTimeSeries = TimeSeries<string>;


export interface DataTypeMap extends Record<DataType, Record<StorageType, any>> {
    Unknown: {
        Single: never,
        Array: never,
        TimeSeries: never
    };
    Boolean: {
        Single: boolean;
        Array: never;
        TimeSeries: never;
    };
    Currency: {
        Single: number;
        Array: number[];
        TimeSeries: NumberTimeSeries[];
    };
    Date: {
        Single: Date;
        Array: Date[];
        TimeSeries: never;
    };
    Decimal: {
        Single: number;
        Array: number[];
        TimeSeries: NumberTimeSeries[];
    };
    Enumeration: {
        Single: string;
        Array: string[];
        TimeSeries: never;
    };
    File: {
        Single: string;
        Array: string[];
        TimeSeries: never;
    };
    Integer: {
        Single: number;
        Array: number[];
        TimeSeries: NumberTimeSeries[];
    };
    Link: {
        Single: string;
        Array: string[];
        TimeSeries: never;
    };
    LongText: {
        Single: string;
        Array: never;
        TimeSeries: never;
    };
    String: {
        Single: string;
        Array: string[];
        TimeSeries: StringTimeSeries[];
    };
}

export interface InputDataTypeMap extends Record<DataType, Record<StorageType, any>> {
    Unknown: {
        Single: never,
        Array: never,
        TimeSeries: never
    };
    Boolean: {
        Single: boolean;
        Array: never;
        TimeSeries: never;
    };
    Currency: {
        Single: number;
        Array: number[];
        TimeSeries: NumberTimeSeries[];
    };
    Date: {
        Single: Date;
        Array: Date[];
        TimeSeries: never;
    };
    Decimal: {
        Single: number;
        Array: number[];
        TimeSeries: NumberTimeSeries[];
    };
    Enumeration: {
        Single: string;
        Array: string[];
        TimeSeries: never;
    };
    File: {
        Single: FileViewValue;
        Array: FileViewValue[];
        TimeSeries: never;
    };
    Integer: {
        Single: number;
        Array: number[];
        TimeSeries: NumberTimeSeries[];
    };
    Link: {
        Single: string;
        Array: string[];
        TimeSeries: never;
    };
    LongText: {
        Single: string;
        Array: never;
        TimeSeries: never;
    };
    String: {
        Single: string;
        Array: string[];
        TimeSeries: StringTimeSeries[];
    };
}

export const storageTypesByDataType = new Map<DataType, StorageType[]>([
    ["Unknown", []],
    ["Boolean", ["Single"]],
    ["Currency", ["Single", "Array", "TimeSeries"]],
    ["Date", ["Single", "Array"]],
    ["Decimal", ["Single", "Array", "TimeSeries"]],
    ["Enumeration", ["Single", "Array"]],
    ["File", ["Single", "Array"]],
    ["Integer", ["Single", "Array", "TimeSeries"]],
    ["Link", ["Single", "Array"]],
    ["LongText", ["Single", "Array"]],
    ["String", ["Single", "Array"]]
]);