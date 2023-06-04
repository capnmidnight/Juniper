import { TypedEvent } from "@juniper-lib/events/EventBase";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
