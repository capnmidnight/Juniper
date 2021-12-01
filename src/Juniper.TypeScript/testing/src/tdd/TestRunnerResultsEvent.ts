import { TypedEvent } from "juniper-tslib";
import { TestScore } from "./TestScore";

export type TestResults = Map<string, Map<string, TestScore>>;

/**
 * An Event that encapsulates a test result.
 **/
export class TestRunnerResultsEvent extends TypedEvent<"testrunnerresults"> {
    /**
     * Creates a new test result event containing the results.
     */
    constructor(public readonly results: TestResults) {
        super("testrunnerresults");
    }
}
