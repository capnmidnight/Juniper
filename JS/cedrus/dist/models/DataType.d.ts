/**
 * Make sure this stays in sync with the DataType enum in ../../../NETCore/Cedrus/Entities/DataType.cs
 */
import { FileViewValue } from "@juniper-lib/widgets";
export type StorageType = "Single" | "Array" | "TimeSeries";
export declare const StorageTypeValues: StorageType[];
export declare function isStorageType(value: string): value is StorageType;
export type DataType = "Unknown" | "Boolean" | "Integer" | "Decimal" | "Currency" | "String" | "Enumeration" | "LongText" | "Date" | "Link" | "File";
export declare const DataTypeValues: DataType[];
export declare function isDataType(value: string): value is DataType;
export declare function hasDataType(obj: unknown, dataType?: DataType): boolean;
export declare function hasStorageType(obj: unknown, storageType?: StorageType): boolean;
export type TimeSeries<T> = {
    date: Date;
    value: T;
};
export type NumberTimeSeries = TimeSeries<number>;
export type StringTimeSeries = TimeSeries<string>;
export interface DataTypeMap extends Record<DataType, Record<StorageType, any>> {
    Unknown: {
        Single: never;
        Array: never;
        TimeSeries: never;
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
        Single: never;
        Array: never;
        TimeSeries: never;
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
export declare const storageTypesByDataType: Map<DataType, StorageType[]>;
//# sourceMappingURL=DataType.d.ts.map