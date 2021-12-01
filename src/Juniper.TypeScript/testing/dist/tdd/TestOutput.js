import { TypedEventBase } from "juniper-tslib";
import { TestOutputResultsEvent } from "./TestOutputResultsEvent";
import { TestRunner } from "./TestRunner";
import { TestStates } from "./TestStates";
export class TestOutput extends TypedEventBase {
    rest;
    constructor(...rest) {
        super();
        this.rest = rest;
    }
    /**
     * Runs a specific test within a test case.
     */
    async run(caseName, testName) {
        const testRunner = new TestRunner(...this.rest);
        testRunner.addEventListener("testrunnerresults", (/** @type {TestRunnerResultsEvent} */ evt) => {
            const results = evt.results;
            let totalFound = 0, totalRan = 0, totalCompleted = 0, totalIncomplete = 0, totalSucceeded = 0, totalFailed = 0;
            for (let testCase of results.values()) {
                for (let test of testCase.values()) {
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
        testRunner.run(caseName, testName);
    }
}
