import { TypedEvent } from "juniper-tslib";
export class TestCaseFailEvent extends TypedEvent {
    message;
    constructor(message) {
        super("testcasefail");
        this.message = message;
    }
}
