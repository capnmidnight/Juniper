import { TypedEvent } from "@juniper-lib/events";
export class ActivityEvent extends TypedEvent {
    constructor() {
        super("activity");
        this.level = 0;
    }
}
//# sourceMappingURL=ActivityEvent.js.map