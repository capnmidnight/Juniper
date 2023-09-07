import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
export declare class TestCaseFailEvent extends TypedEvent<"testcasefail"> {
    readonly message: string;
    constructor(message: string);
}
//# sourceMappingURL=TestCaseFailEvent.d.ts.map