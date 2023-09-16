import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
