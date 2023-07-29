import { TypedEvent } from "@juniper-lib/events/TypedEventBase";

export class TestCaseFailEvent extends TypedEvent<"testcasefail"> {
    constructor(public readonly message: string) {
        super("testcasefail");
    }
}
