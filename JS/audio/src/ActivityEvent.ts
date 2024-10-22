import { TypedEvent } from "@juniper-lib/events";



export class ActivityEvent extends TypedEvent<"activity"> {
    public level = 0;
    constructor() {
        super("activity");
    }
}
