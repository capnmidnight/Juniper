import { TypedEvent } from "@juniper-lib/events";
export class TestCaseFailEvent extends TypedEvent {
    constructor(message) {
        super("testcasefail");
        this.message = message;
    }
}
//# sourceMappingURL=TestCaseFailEvent.js.map