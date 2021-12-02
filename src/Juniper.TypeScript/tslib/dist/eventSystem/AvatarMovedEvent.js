import { TypedEvent } from "../events/EventBase";
import { PointerName } from "./PointerName";
export class AvatarMovedEvent extends TypedEvent {
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
    name = PointerName.LocalUser;
    constructor() {
        super("avatarmoved");
    }
    set(px, py, pz, fx, fy, fz, ux, uy, uz, height) {
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
