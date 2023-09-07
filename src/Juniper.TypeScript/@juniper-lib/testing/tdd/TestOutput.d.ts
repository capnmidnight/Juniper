import { TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { TestCaseConstructor } from "./TestCase";
import { TestOutputResultsEvent } from "./TestOutputResultsEvent";
type TestOutputEvents = {
    "testoutputresults": TestOutputResultsEvent;
};
export declare class TestOutput extends TypedEventTarget<TestOutputEvents> {
    private readonly CaseClasses;
    constructor(...CaseClasses: TestCaseConstructor[]);
    /**
     * Runs a specific test within a test case.
     */
    run(caseName?: string, testName?: string): Promise<void>;
    protected scaffold(): void;
    private createTestRunner;
}
export {};
//# sourceMappingURL=TestOutput.d.ts.map