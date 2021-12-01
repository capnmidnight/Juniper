import { TypedEventBase } from "juniper-tslib";
import { CaseClassConstructor } from "./CaseClassConstructor";
import { TestResults, TestRunnerResultsEvent } from "./TestRunnerResultsEvent";
interface TestRunnerEvents {
    testrunnerresults: TestRunnerResultsEvent;
}
export declare class TestRunner extends TypedEventBase<TestRunnerEvents> {
    private readonly props;
    private readonly CaseClasses;
    constructor(...rest: any[]);
    run(testCaseName: string, testName: string): Promise<void>;
    runTest(CaseClass: CaseClassConstructor, funcName: string, results: TestResults, className: string, onUpdate: Function): Promise<void>;
}
export {};
