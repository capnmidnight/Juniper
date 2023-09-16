import { PriorityMap } from "@juniper-lib/collections/dist/PriorityMap";
import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
import { TestScore } from "./TestScore";
export type TestResults = PriorityMap<string, string, TestScore>;
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
//# sourceMappingURL=TestRunnerResultsEvent.d.ts.map