import { Vec2, Vec3, Vec4 } from "gl-matrix/dist/esm";
export declare const RIGHT: Vec3;
export declare const UP: Vec3;
export declare const FWD: Vec3;
export declare const Pi: number;
export declare const HalfPi: number;
export declare const Tau: number;
export declare const TWENTY_FOUR_LOG10 = 55.2620422318571;
export declare const LOG1000 = 6.90775527898214;
export declare const LOG2_DIV2 = 0.346573590279973;
export declare const EPSILON_FLOAT = 1e-8;
export declare const TIME_MAX = 8640000000000000;
export declare const TIME_MIN: number;
/**
 * Find the median of an array of numbers.
 * Assumes the array is sorted.
 * @returns null on an empty array, value of the middle element in an odd-length array, or the midpoint between the middle-most two values of an even-length array.
 **/
export declare function calculateMedian(arr: readonly [number, ...number[]]): number | null;
/**
 * Calculates the arithmetic mean of an array of numbers.
 * @returns null on an empty array.
 **/
export declare function calculateMean(arr: readonly [number, ...number[]]): number | null;
/**
 * Calculates the statistical variance of an array of numbers.
 * Returns null for arrays smaller than 2 elements.
 **/
export declare function calculateVariance(arr: readonly [number, number, ...number[]]): number | null;
/**
 * Calculates the standard deviation of an array of numbers.
 * Returns null for arrays smaller than 2 elements.
 **/
export declare function calculateStandardDeviation(arr: readonly [number, number, ...number[]]): number | null;
export declare function xy2i(x: number, y: number, width: number, components?: number): number;
export declare function vec22i(vec: Vec2, width: number, components?: number): number;
export declare function i2vec2(vec: Vec2, i: number, width: number, components?: number): void;
export declare function radiansClamp(radians: number): number;
export declare function degreesClamp(radians: number): number;
/**
 * Force a value onto a range
 */
export declare function clamp(v: number, min: number, max: number): number;
/**
 * Convert degress to radians
 * @param degrees
 */
export declare function deg2rad(degrees: number): number;
/**
 * Convert radias to degress
 * @param radians
 */
export declare function rad2deg(radians: number): number;
/**
 * Returns the number with the largest magnitude.
 * @param a
 * @param b
 */
export declare function maxly(...numbers: number[]): number;
/**
 * Returns the number with the smallest magnitude.
 * @param numbers
 */
export declare function minly(...numbers: number[]): number;
/**
 * Translate a value into a range.
 */
export declare function project(v: number, min: number, max: number): number;
/**
 * Translate a value out of a range.
 */
export declare function unproject(v: number, min: number, max: number): number;
export declare function formatNumber(value: number, digits?: number): string;
export declare function parseNumber(value: string): number | null;
export declare function formatVolume(value: number): string;
export declare function parseVolume(value: string): number | null;
/**
 * Pick a value that is proportionally between two values.
 */
export declare function lerp(a: number, b: number, p: number): number;
export interface IPoint {
    x: number;
    y: number;
}
export interface ISize {
    width: number;
    height: number;
}
export interface PaddingRect {
    top: number;
    right: number;
    bottom: number;
    left: number;
}
export interface IRectangle extends PaddingRect {
    point: IPoint;
    size: ISize;
    x: number;
    y: number;
}
export declare class Point implements IPoint {
    x: number;
    y: number;
    constructor(x?: number, y?: number);
    set(x: number, y: number): void;
    copy(p: IPoint): void;
    toCell(character: ISize, scroll: IPoint, gridBounds: IRectangle): void;
    inBounds(bounds: IRectangle): boolean;
    clone(): Point;
    toString(): string;
}
export declare class Size implements ISize {
    width: number;
    height: number;
    constructor(width?: number, height?: number);
    set(width: number, height: number): void;
    copy(s: ISize): void;
    clone(): Size;
    toString(): string;
}
export declare class Rectangle implements IRectangle {
    point: Point;
    size: Size;
    constructor(x?: number, y?: number, width?: number, height?: number);
    get x(): number;
    set x(x: number);
    get left(): number;
    set left(x: number);
    get width(): number;
    set width(width: number);
    get right(): number;
    set right(right: number);
    get y(): number;
    set y(y: number);
    get top(): number;
    set top(y: number);
    get height(): number;
    set height(height: number);
    get bottom(): number;
    set bottom(bottom: number);
    get area(): number;
    set(x: number, y: number, width: number, height: number): void;
    copy(r: IRectangle): void;
    clone(): Rectangle;
    overlap(r: IRectangle): Rectangle | null;
    toString(): string;
}
export declare function isPowerOf2(v: number): boolean;
export declare function nextPowerOf2(v: number): number;
export declare function prevPowerOf2(v: number): number;
export declare function closestPowerOf2(v: number): number;
export declare function truncate(v: number): number;
type Vec = Vec2 | Vec3 | Vec4;
export declare function warnOnNaN(val: number | number[] | Vec, msg?: string): void;
export {};
//# sourceMappingURL=math.d.ts.map