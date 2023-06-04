import { TypedEvent } from "@juniper-lib/events/EventBase";

export class TestCaseMessageEvent extends TypedEvent<"testcasemessage"> {
    constructor(public readonly message: string) {
        super("testcasemessage");
    }
}
