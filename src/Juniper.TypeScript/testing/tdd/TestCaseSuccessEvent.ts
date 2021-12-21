import { TypedEvent } from "juniper-tslib";

export class TestCaseSuccessEvent extends TypedEvent<"testcasesuccess"> {
    constructor() {
        super("testcasesuccess");
    }
}
