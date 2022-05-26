import { TypedEvent } from "@juniper-lib/tslib";

export class FlickEvent extends TypedEvent<"flick"> {
    constructor(public direction: number) {
        super("flick");
    }
}