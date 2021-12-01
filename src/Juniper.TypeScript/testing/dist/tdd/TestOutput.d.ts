import { TypedEventBase } from "juniper-tslib";
import { TestOutputResultsEvent } from "./TestOutputResultsEvent";
interface TestOutputEvents {
    "testoutputresults": TestOutputResultsEvent;
}
export declare class TestOutput extends TypedEventBase<TestOutputEvents> {
    private readonly rest;
    constructor(...rest: any[]);
    /**
     * Runs a specific test within a test case.
     */
    run(caseName?: string, testName?: string): Promise<void>;
}
export {};
