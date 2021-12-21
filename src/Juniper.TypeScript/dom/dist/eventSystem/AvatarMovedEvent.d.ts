import { TypedEvent } from "juniper-tslib";
import { PointerName } from "./PointerName";
export declare class AvatarMovedEvent extends TypedEvent<"avatarmoved"> {
    px: number;
    py: number;
    pz: number;
    fx: number;
    fy: number;
    fz: number;
    ux: number;
    uy: number;
    uz: number;
    height: number;
    readonly name: PointerName;
    constructor();
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number): void;
}
