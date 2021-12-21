import { TypedEvent } from "juniper-tslib";
export class FlickEvent extends TypedEvent {
    direction;
    constructor(direction) {
        super("flick");
        this.direction = direction;
    }
}
