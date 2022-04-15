import { TypedEvent } from "@juniper/events";

export class TestCaseFailEvent extends TypedEvent<"testcasefail"> {
    constructor(public readonly message: string) {
        super("testcasefail");
    }
}
