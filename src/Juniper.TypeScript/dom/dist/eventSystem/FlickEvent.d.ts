import { TypedEvent } from "juniper-tslib";
export declare class FlickEvent extends TypedEvent<"flick"> {
    direction: number;
    constructor(direction: number);
}
