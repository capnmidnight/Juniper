import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";

export class TestCaseFailEvent extends TypedEvent<"testcasefail"> {
    constructor(public readonly message: string) {
        super("testcasefail");
    }
}
