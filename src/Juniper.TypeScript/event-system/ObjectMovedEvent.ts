import { TypedEvent } from "@juniper/events";
import { PointerName } from "./PointerName";

export class ObjectMovedEvent extends TypedEvent<"objectMoved"> {
    px = 0;
    py = 0;
    pz = 0;
    fx = 0;
    fy = 0;
    fz = 0;
    ux = 0;
    uy = 0;
    uz = 0;

    constructor(public name: PointerName = null) {
        super("objectMoved");
    }

    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number) {
        this.px = px;
        this.py = py;
        this.pz = pz;
        this.fx = fx;
        this.fy = fy;
        this.fz = fz;
        this.ux = ux;
        this.uy = uy;
        this.uz = uz;
    }
}