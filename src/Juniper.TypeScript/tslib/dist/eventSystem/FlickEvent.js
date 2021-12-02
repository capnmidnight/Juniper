import { TypedEvent } from "../events/EventBase";
export class FlickEvent extends TypedEvent {
    direction;
    constructor(direction) {
        super("flick");
        this.direction = direction;
    }
}
