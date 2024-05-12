import { TypedEvent } from "./TypedEventTarget";

export class RefreshEvent extends TypedEvent<"refresh"> {
    constructor() {
        super("refresh");
    }
}
