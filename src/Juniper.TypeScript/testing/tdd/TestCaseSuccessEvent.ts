import { TypedEvent } from "@juniper/events";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
