import type { IRectangle } from "./Rectangle";
import type { ISize } from "./Size";
export interface IPoint {
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
