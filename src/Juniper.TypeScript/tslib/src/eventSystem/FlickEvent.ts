import { TypedEvent } from "../events/EventBase";

export class FlickEvent extends TypedEvent<"flick"> {
    constructor(public direction: number) {
        super("flick");
    }
}