import { TypedEvent } from "juniper-tslib";
import { PointerName } from "./PointerName";
export declare class ObjectMovedEvent extends TypedEvent<"objectMoved"> {
    name: PointerName;
    px: number;
    py: number;
    pz: number;
    fx: number;
    fy: number;
    fz: number;
    ux: number;
    uy: number;
    uz: number;
    constructor(name?: PointerName);
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
}
