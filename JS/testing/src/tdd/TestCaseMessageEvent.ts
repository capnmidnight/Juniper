import { TypedEvent } from "@juniper-lib/events";

export class TestCaseMessageEvent extends TypedEvent<"testcasemessage"> {
    constructor(public readonly message: string) {
        super("testcasemessage");
    }
}
