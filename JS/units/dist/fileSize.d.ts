type Base = 2 | 10;
type Base2Units = "KiB" | "MiB" | "GiB" | "TiB" | "PiB";
type Base10Units = "KB" | "MB" | "GB" | "TB" | "PB";
type Units = "B" | Base2Units | Base10Units;
/**
 * Converts a value in number of bytes to a value in base-2 or
 * base-10 memory units, e.g. KiB (kibibytes, 2^10) vs KB (kilobytes, bayse 10^3)
 * and adds the unit specifier to the string. The exact label that is used
 * is the largest label that results in the smallest, greater than 0
 * integer part of the number.
 * @param value the memory in bytes
 * @param units the unit of memory to convert to
 */
export declare function formatBytes(value: number, base?: Base): string;
/**
 * Converts a value in number of bytes to a value in base-2 or
 * base-10 memory units, e.g. KiB (kibibytes, 2^10) vs KB (kilobytes, bayse 10^3).
 * @param value the memory in bytes
 * @param units the unit of memory to convert to
 */
export declare function fromBytes(value: number, units: Units): number;
/**
 * Converts a value in a number of bytes expressed as a base-2 or
 * base-10 memory units (e.g. KiB (kibibytes, 2^10) vs KB (kilobytes, bayse 10^3))
 * to a number of bytes.
 * @param value the value in base-2 or base-10 memory units
 * @param units the units to convert from
 * @returns
 */
export declare function toBytes(value: number, units: Units): number;
export {};
//# sourceMappingURL=fileSize.d.ts.map