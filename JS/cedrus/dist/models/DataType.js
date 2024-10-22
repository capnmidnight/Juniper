/**
 * Make sure this stays in sync with the DataType enum in ../../../NETCore/Cedrus/Entities/DataType.cs
 */
import { isNullOrUndefined, isObject, isString } from "@juniper-lib/util";
export const StorageTypeValues = [
    "Single",
    "Array",
    "TimeSeries"
];
const storageTypes = new Set(StorageTypeValues);
export function isStorageType(value) {
    return storageTypes.has(value);
}
export const DataTypeValues = [
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
export function isDataType(value) {
    return dataTypes.has(value);
}
export function hasDataType(obj, dataType) {
    return isObject(obj)
        && "type" in obj
        && isString(obj.type)
        && (isNullOrUndefined(dataType)
            || obj.type === dataType);
}
export function hasStorageType(obj, storageType) {
    return isObject(obj)
        && "storage" in obj
        && isString(obj.storage)
        && (isNullOrUndefined(storageType)
            || obj.storage === storageType);
}
export const storageTypesByDataType = new Map([
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
//# sourceMappingURL=DataType.js.map