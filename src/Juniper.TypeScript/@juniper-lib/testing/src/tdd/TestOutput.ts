import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { TestCaseConstructor } from "./TestCase";
import { TestOutputResultsEvent, TestStats } from "./TestOutputResultsEvent";
import { TestRunner } from "./TestRunner";
import { TestStates } from "./TestStates";

type TestOutputEvents = {
    "testoutputresults": TestOutputResultsEvent;
}

export class TestOutput extends TypedEventTarget<TestOutputEvents> {
    private readonly CaseClasses: any[];

    constructor(...CaseClasses: TestCaseConstructor[]) {
        super();
        this.CaseClasses = CaseClasses;
    }

    /**
     * Runs a specific test within a test case.
     */
    async run(caseName?: string, testName?: string) {
        const testRunner = this.createTestRunner();
        testRunner.run(caseName, testName);
    }

    protected scaffold() {
        const testRunner = this.createTestRunner();
        testRunner.scaffold();
    }

    private createTestRunner() {
        const testRunner = new TestRunner(...this.CaseClasses);
        testRunner.addEventListener("testrunnerresults", (evt) => {
            const results = evt.results;
            let totalFound = 0, totalRan = 0, totalCompleted = 0, totalIncomplete = 0, totalSucceeded = 0, totalFailed = 0;
            for (const test of results.values()) {
                ++totalFound;
                if (test.state & TestStates.started) {
                    ++totalRan;
                }
                if (test.state & TestStates.completed) {
                    ++totalCompleted;
                }
                else {
                    ++totalIncomplete;
                }
                if (test.state & TestStates.succeeded) {
                    ++totalSucceeded;
                }
                if (test.state & TestStates.failed) {
                    ++totalFailed;
                }
            }

            const stats: TestStats = {
                totalFound,
                totalRan,
                totalCompleted,
                totalIncomplete,
                totalSucceeded,
                totalFailed
            };
            this.dispatchEvent(new TestOutputResultsEvent(results, stats));
        });
        return testRunner;
    }
}
