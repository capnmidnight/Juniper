import { TypedEvent } from "juniper-tslib";
import { TestScore } from "./TestScore";
export declare type TestResults = Map<string, Map<string, TestScore>>;
/**
 * An Event that encapsulates a test result.
 **/
export declare class TestRunnerResultsEvent extends TypedEvent<"testrunnerresults"> {
    readonly results: TestResults;
    /**
     * Creates a new test result event containing the results.
     */
    constructor(results: TestResults);
}
