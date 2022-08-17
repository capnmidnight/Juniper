import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
