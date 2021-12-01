import { TypedEvent } from "juniper-tslib";
export declare class TestCaseMessageEvent extends TypedEvent<"testcasemessage"> {
    readonly message: string;
    constructor(message: string);
}
