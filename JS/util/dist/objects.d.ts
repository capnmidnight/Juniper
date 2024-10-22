import { Diff } from "./arrays";
/**
 * Checks to see if two objects contain the same elements
 * @returns null if the objects match, a diff object, otherwise.
 */
export declare function objectCompare(a: object, b: object): Diff;
/**
 * Determines if an object contains a function at the given field name.
 * These are *usually* methods.
 */
export declare function hasMethod(obj: object, name: string): boolean;
/**
 * Attempts to parse a string value to an object, but returns null instead of throwing an error on failure.
 */
export declare function tryParse<T>(json: string): T;
/**
 * For use with enumerations: returns the maximum numeric value the enumeration defines.
 * @param x
 * @returns
 */
export declare function maxEnumValue(x: object): number;
export declare function objectSelect(obj: any, fieldDef: string): unknown;
//# sourceMappingURL=objects.d.ts.map