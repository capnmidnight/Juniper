import { TypedEvent } from "@juniper/events";

export class FlickEvent extends TypedEvent<"flick"> {
    constructor(public direction: number) {
        super("flick");
    }
}