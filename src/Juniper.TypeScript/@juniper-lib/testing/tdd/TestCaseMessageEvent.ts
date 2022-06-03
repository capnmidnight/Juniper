import { TypedEvent } from "@juniper-lib/tslib";

export class TestCaseMessageEvent extends TypedEvent<"testcasemessage"> {
    constructor(public readonly message: string) {
        super("testcasemessage");
    }
}
