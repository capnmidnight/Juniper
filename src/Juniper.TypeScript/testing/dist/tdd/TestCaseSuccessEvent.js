import { TypedEvent } from "juniper-tslib";
export class TestCaseSuccessEvent extends TypedEvent {
    constructor() {
        super("testcasesuccess");
    }
}
