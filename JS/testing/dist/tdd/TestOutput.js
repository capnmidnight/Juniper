import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { TestOutputResultsEvent } from "./TestOutputResultsEvent";
import { TestRunner } from "./TestRunner";
import { TestStates } from "./TestStates";
export class TestOutput extends TypedEventTarget {
    constructor(...CaseClasses) {
        super();
        this.CaseClasses = CaseClasses;
    }
    /**
     * Runs a specific test within a test case.
     */
    async run(caseName, testName) {
        const testRunner = this.createTestRunner();
        testRunner.run(caseName, testName);
    }
    scaffold() {
        const testRunner = this.createTestRunner();
        testRunner.scaffold();
    }
    createTestRunner() {
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
            const stats = {
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
//# sourceMappingURL=TestOutput.js.map