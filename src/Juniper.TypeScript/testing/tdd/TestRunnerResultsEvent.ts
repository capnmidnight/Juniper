import { PriorityMap } from "@juniper/collections";
import { TypedEvent } from "@juniper/events";
import { TestScore } from "./TestScore";

export type TestResults = PriorityMap<string, string, TestScore>;

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
