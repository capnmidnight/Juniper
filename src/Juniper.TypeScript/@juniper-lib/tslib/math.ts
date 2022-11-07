import { vec2, vec3 } from "gl-matrix";
import { isDefined, isGoodNumber, isNumber } from "./typeChecks";

export const RIGHT = /*@__PURE__*/ vec3.fromValues(1, 0, 0);
export const UP = /*@__PURE__*/ vec3.fromValues(0, 1, 0);
export const FWD = /*@__PURE__*/ vec3.fromValues(0, 0, -1);
export const Pi =/*@__PURE__*/ Math.PI;
export const HalfPi =/*@__PURE__*/ 0.5 * Pi;
export const Tau = /*@__PURE__*/ 2 * Pi;

export function xy2i(x: number, y: number, width: number, components: number = 1): number {
    return components * (x + width * y);
}

export function vec22i(vec: vec2, width: number, components: number = 1): number {
    return xy2i(vec[0], vec[1], width, components);
}

export function i2vec2(vec: vec2, i: number, width: number, components: number = 1): void {
    const stride = width * components;
    const p = i % stride;
    const x = Math.floor(p / components);
    const y = Math.floor(i / stride);
    vec2.set(vec, x, y);
}

export function radiansClamp(radians: number) {
    return ((radians % Tau) + Tau) % Tau;
}

export function degreesClamp(radians: number) {
    return ((radians % 360) + 360) % 360;
}

/**
 * Force a value onto a range
 */
export function clamp(v: number, min: number, max: number) {
    return Math.min(max, Math.max(min, v));
}

/**
 * Convert degress to radians
 * @param degrees
 */
export function deg2rad(degrees: number) {
    return degrees * Tau / 360;
}

/**
 * Convert radias to degress
 * @param radians
 */
export function rad2deg(radians: number) {
    return radians * 360 / Tau;
}

/**
 * Returns the number with the largest magnitude.
 * @param a
 * @param b
 */
export function maxly(...numbers: number[]) {
    let max = 0;
    for (const n of numbers) {
        if (Math.abs(n) > max) {
            max = n;
        }
    }

    return max;
}

/**
 * Returns the number with the smallest magnitude.
 * @param numbers
 */
export function minly(...numbers: number[]) {
    let min = Number.MAX_VALUE;
    for (const n of numbers) {
        if (Math.abs(n) < min) {
            min = n;
        }
    }

    return min;
}

/**
 * Translate a value into a range.
 */
export function project(v: number, min: number, max: number) {
    const delta = max - min;
    if (delta === 0) {
        return 0;
    }
    else {
        return (v - min) / delta;
    }
}


/**
 * Translate a value out of a range.
 */

export function unproject(v: number, min: number, max: number) {
    return v * (max - min) + min;
}


export function formatNumber(value: number, digits = 0): string {
    if (isNumber(value)) {
        return value.toFixed(digits);
    }
    else {
        return "";
    }
}

export function parseNumber(value: string): number {
    if (/\d+/.test(value)) {
        return parseFloat(value);
    }
    else {
        return null;
    }
}

export function formatVolume(value: number): string {
    if (isNumber(value)) {
        return clamp(unproject(value, 0, 100), 0, 100).toFixed(0);
    }
    else {
        return "";
    }
}

export function parseVolume(value: string): number {
    if (/\d+/.test(value)) {
        return clamp(project(parseInt(value, 10), 0, 100), 0, 1);
    }
    else {
        return null;
    }
}

/**
 * Pick a value that is proportionally between two values.
 */
export function lerp(a: number, b: number, p: number) {
    return (1 - p) * a + p * b;
}

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

export class Point implements IPoint {
    constructor(public x: number = 0, public y: number = 0) {
        Object.seal(this);
    }

    set(x: number, y: number) {
        this.x = x;
        this.y = y;
    }

    copy(p: IPoint) {
        if (isDefined(p)) {
            this.x = p.x;
            this.y = p.y;
        }
    }

    toCell(character: ISize, scroll: IPoint, gridBounds: IRectangle) {
        this.x = Math.round(this.x / character.width) + scroll.x - gridBounds.x;
        this.y = Math.floor((this.y / character.height) - 0.25) + scroll.y;
    }

    inBounds(bounds: IRectangle) {
        return bounds.left <= this.x
            && this.x < bounds.right
            && bounds.top <= this.y
            && this.y < bounds.bottom;
    }

    clone() {
        return new Point(this.x, this.y);
    }

    toString() {
        return `(x:${this.x}, y:${this.y})`;
    }
}

export class Size implements ISize {
    constructor(public width = 0, public height = 0) {
        Object.seal(this);
    }

    set(width: number, height: number) {
        this.width = width;
        this.height = height;
    }

    copy(s: ISize) {
        if (isDefined(s)) {
            this.width = s.width;
            this.height = s.height;
        }
    }

    clone() {
        return new Size(this.width, this.height);
    }

    toString() {
        return `<w:${this.width}, h:${this.height}>`;
    }
}

export class Rectangle implements IRectangle {
    point: Point;
    size: Size;
    constructor(x: number = 0, y: number = 0, width: number = 0, height: number = 0) {
        this.point = new Point(x, y);
        this.size = new Size(width, height);
        Object.freeze(this);
    }

    get x() {
        return this.point.x;
    }

    set x(x) {
        this.point.x = x;
    }

    get left() {
        return this.point.x;
    }
    set left(x) {
        this.point.x = x;
    }

    get width() {
        return this.size.width;
    }
    set width(width) {
        this.size.width = width;
    }

    get right() {
        return this.point.x + this.size.width;
    }
    set right(right) {
        this.point.x = right - this.size.width;
    }

    get y() {
        return this.point.y;
    }
    set y(y) {
        this.point.y = y;
    }

    get top() {
        return this.point.y;
    }
    set top(y) {
        this.point.y = y;
    }

    get height() {
        return this.size.height;
    }
    set height(height) {
        this.size.height = height;
    }

    get bottom() {
        return this.point.y + this.size.height;
    }
    set bottom(bottom) {
        this.point.y = bottom - this.size.height;
    }

    get area() {
        return this.width * this.height;
    }

    set(x: number, y: number, width: number, height: number) {
        this.point.set(x, y);
        this.size.set(width, height);
    }

    copy(r: IRectangle) {
        if (isDefined(r)) {
            this.point.copy(r.point);
            this.size.copy(r.size);
        }
    }

    clone() {
        return new Rectangle(this.point.x, this.point.y, this.size.width, this.size.height);
    }

    overlap(r: IRectangle) {
        const left = Math.max(this.left, r.left),
            top = Math.max(this.top, r.top),
            right = Math.min(this.right, r.right),
            bottom = Math.min(this.bottom, r.bottom);
        if (right > left && bottom > top) {
            return new Rectangle(left, top, right - left, bottom - top);
        }
        else {
            return null;
        }
    }

    toString() {
        return `[${this.point.toString()} x ${this.size.toString()}]`;
    }
}

export function isPowerOf2(v: number) {
    return ((v != 0) && !(v & (v - 1)));
}

export function nextPowerOf2(v: number) {
    return Math.pow(2, Math.ceil(Math.log2(v)));
}

export function prevPowerOf2(v: number) {
    return Math.pow(2, Math.floor(Math.log2(v)));
}

export function closestPowerOf2(v: number) {
    return Math.pow(2, Math.round(Math.log2(v)));
}

export function truncate(v: number): number {
    if (Math.abs(v) > 0.0001) {
        return v;
    }

    return 0;
}

interface Vec2 { x: number; y: number; }
interface Vec3 extends Vec2 { z: number }
interface Vec4 extends Vec3 { w: number }
type Vec = Vec2 | Vec3 | Vec4 | ArrayLike<number>;

export function warnOnNaN(val: number | Vec, msg?: string): void {
    let type: string = null;
    let isBad = false;

    if (isNumber(val)) {
        type = "Value is"
        isBad = !isGoodNumber(val);
    }
    else if ("length" in val) {
        type = "Array contains";
        for (let i = 0; i < val.length; ++i) {
            if (!isGoodNumber(val[i])) {
                isBad = true;
                break;
            }
        }
    }
    else {
        type = "Vector component";
        if ("w" in val) {
            isBad = isBad || !isGoodNumber(val.w);
        }

        if ("z" in val) {
            isBad = isBad || !isGoodNumber(val.z);
        }

        isBad = isBad || !isGoodNumber(val.y);
        isBad = isBad || !isGoodNumber(val.x);
    }


    if (isBad) {
        if (msg) {
            msg = `[${msg}] `
        }
        else {
            msg = "";
        }

        console.warn(`${msg}${type} not-a-number`);
    }
}
