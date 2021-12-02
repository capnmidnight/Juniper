import { TypedEvent } from "../events/EventBase";
export class ObjectMovedEvent extends TypedEvent {
    name;
    px = 0;
    py = 0;
    pz = 0;
    fx = 0;
    fy = 0;
    fz = 0;
    ux = 0;
    uy = 0;
    uz = 0;
    constructor(name = null) {
        super("objectMoved");
        this.name = name;
    }
    set(px, py, pz, fx, fy, fz, ux, uy, uz) {
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
