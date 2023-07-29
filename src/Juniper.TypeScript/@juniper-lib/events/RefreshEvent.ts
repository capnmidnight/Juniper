import { TypedEvent } from "./TypedEventBase";

export class RefreshEvent extends TypedEvent<"refresh"> {
    constructor() {
        super("refresh");
    }
}
