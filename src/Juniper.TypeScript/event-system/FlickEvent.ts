import { TypedEvent } from "@juniper/tslib";

export class FlickEvent extends TypedEvent<"flick"> {
    constructor(public direction: number) {
        super("flick");
    }
}