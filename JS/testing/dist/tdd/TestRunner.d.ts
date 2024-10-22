import { TypedEventTarget } from "@juniper-lib/events";
import type { TestCaseConstructor } from "./TestCase";
import { TestRunnerResultsEvent } from "./TestRunnerResultsEvent";
type TestRunnerEvents = {
    testrunnerresults: TestRunnerResultsEvent;
};
export declare class TestRunner extends TypedEventTarget<TestRunnerEvents> {
    private readonly props;
    private readonly CaseClasses;
    constructor(...rest: TestCaseConstructor[]);
    scaffold(): void;
    run(testCaseName: string, testName: string): Promise<void>;
    private runTest;
}
export {};
//# sourceMappingURL=TestRunner.d.ts.map