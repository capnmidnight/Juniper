import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
