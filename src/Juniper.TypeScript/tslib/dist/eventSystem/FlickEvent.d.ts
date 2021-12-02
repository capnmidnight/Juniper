import { TypedEvent } from "../events/EventBase";
export declare class FlickEvent extends TypedEvent<"flick"> {
    direction: number;
    constructor(direction: number);
}
