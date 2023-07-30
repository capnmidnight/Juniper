import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";

export class TestCaseMessageEvent extends TypedEvent<"testcasemessage"> {
    constructor(public readonly message: string) {
        super("testcasemessage");
    }
}
