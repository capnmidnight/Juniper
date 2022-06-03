import { TypedEvent } from "@juniper-lib/tslib";
import { PointerName } from "./PointerName";

export class AvatarMovedEvent extends TypedEvent<"avatarmoved"> {
    px = 0;
    py = 0;
    pz = 0;
    fx = 0;
    fy = 0;
    fz = 0;
    ux = 0;
    uy = 0;
    uz = 0;
    height = 0;

    public readonly name: PointerName = PointerName.LocalUser;

    constructor() {
        super("avatarmoved");
    }

    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number, height: number) {
        this.px = px;
        this.py = py;
        this.pz = pz;
        this.fx = fx;
        this.fy = fy;
        this.fz = fz;
        this.ux = ux;
        this.uy = uy;
        this.uz = uz;
        this.height = height;
    }
}
