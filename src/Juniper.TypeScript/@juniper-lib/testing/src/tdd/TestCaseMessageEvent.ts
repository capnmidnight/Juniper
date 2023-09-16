import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";

export class TestCaseMessageEvent extends TypedEvent<"testcasemessage"> {
    constructor(public readonly message: string) {
        super("testcasemessage");
    }
}
