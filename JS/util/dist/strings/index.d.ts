export * from "./stringRandom";
export * from "./stringRepeat";
export * from "./stringReverse";
export * from "./stringSearch";
export * from "./stringSimilarity";
export * from "./stringToName";
/**
* Calls the `trim` method on a string. Useful for functional application.
*/
export declare function stringTrim(v: string): string;
/**
 * Calls the `toUpperCase` method on a string. Useful for functional application.
 */
export declare function toUpper(str: string): string;
/**
 * Calls the `toLowerCase` method on a string. Useful for functional application.
 */
export declare function toLower(str: string): string;
/**
 * Calls the `toString` method on an object. Useful for functional application.
 */
export declare function toString<T = object>(obj: T): string;
/**
 * Creats a callback function that calls the `split` method on an object. Useful for functional application.
 */
export declare function split(delim: string | RegExp, limit?: number): (str: string) => string[];
/**
 * Formats an email address into a link.
 */
export declare function makeEmailLink(address: string, missingLabel?: string): string;
/**
 * Format a name in first/last format.
 */
export declare function formatNameFirstLast(firstNameStr: string, lastNameStr: string): string;
/**
 * Format a name in last/first format.
 */
export declare function formatNameLastFirst(firstNameStr: string, lastNameStr: string): string;
/**
 * Names in the data files come in a variety of formats.
 *  - {firstname} {lastname}
 *  - {lastname}, {firstname}
 *  - {lastname}, {title};{firstname}
 *  - etc.
 * With some bad habits regarding whitespace.
 * This function attempts to understand the name that
 * has been provided and reformat it into a single format
 * that can better be used for lookups.
 */
export declare function normalizeNameLastFirst(name: string, stripTitle?: boolean): string;
export declare function formatPhoneNumber(countryCode: string, areaCode: string, exchange: string, extension: string): string;
export declare function splitPhoneNumber(number: string): string[];
export declare function checkUnknownValue(data: string, inputType: string): string;
export declare function leftPad(v: string, l: number, c: any): string;
export declare function rightPad(v: string, l: number, c: any): string;
/**
 * Convert three-value logic strings to three-value boolean values.
 * Useful for use with radio buttons
 */
export declare function yesNoToBool(value: string): boolean;
/**
 * Convert three-value boolean values to three-value logic strings.
 * Useful for use with radio buttons
 */
export declare function boolToYesNo(value: boolean): "" | "yes" | "no";
//# sourceMappingURL=index.d.ts.map