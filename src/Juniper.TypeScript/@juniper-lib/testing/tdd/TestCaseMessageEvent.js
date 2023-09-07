import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
export class TestCaseMessageEvent extends TypedEvent {
    constructor(message) {
        super("testcasemessage");
        this.message = message;
    }
}
//# sourceMappingURL=TestCaseMessageEvent.js.map