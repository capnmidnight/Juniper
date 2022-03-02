import { TypedEventBase } from "juniper-tslib";
import { TestOutputResultsEvent, TestStats } from "./TestOutputResultsEvent";
import { TestRunner } from "./TestRunner";
import { TestStates } from "./TestStates";

interface TestOutputEvents {
    "testoutputresults": TestOutputResultsEvent;
}

export class TestOutput extends TypedEventBase<TestOutputEvents> {
    private readonly rest: any[];

    constructor(...rest: any[]) {
        super();
        this.rest = rest;
    }

    /**
     * Runs a specific test within a test case.
     */
    async run(caseName?: string, testName?: string) {
        const testRunner = new TestRunner(...this.rest);
        testRunner.addEventListener("testrunnerresults", (/** @type {TestRunnerResultsEvent} */evt) => {
            const results = evt.results;
            let totalFound = 0,
                totalRan = 0,
                totalCompleted = 0,
                totalIncomplete = 0,
                totalSucceeded = 0,
                totalFailed = 0;
            for (let test of results.values()) {
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
        testRunner.run(caseName, testName);
    }
}
