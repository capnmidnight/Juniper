import type { IPoint } from "./Point";
import { Point } from "./Point";
import type { ISize } from "./Size";
import { Size } from "./Size";
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
    overlap(r: IRectangle): Rectangle;
    toString(): string;
}
