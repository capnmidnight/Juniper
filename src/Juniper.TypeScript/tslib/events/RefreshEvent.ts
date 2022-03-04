import { TypedEvent } from "./EventBase";

export class RefreshEvent extends TypedEvent<"refresh"> {
    constructor() {
        super("refresh");
    }
}
