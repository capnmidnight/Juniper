import { TypedEvent } from "juniper-tslib";
export declare class TestCaseFailEvent extends TypedEvent<"testcasefail"> {
    readonly message: string;
    constructor(message: string);
}
