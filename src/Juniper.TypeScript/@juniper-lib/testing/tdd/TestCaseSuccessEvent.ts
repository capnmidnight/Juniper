import { TypedEvent } from "@juniper-lib/events/TypedEventBase";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
