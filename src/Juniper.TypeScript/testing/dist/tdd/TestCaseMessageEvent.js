import { TypedEvent } from "juniper-tslib";
export class TestCaseMessageEvent extends TypedEvent {
    message;
    constructor(message) {
        super("testcasemessage");
        this.message = message;
    }
}
