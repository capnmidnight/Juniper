import { TypedEvent } from "juniper-tslib";
/**
 * An Event that encapsulates a test result.
 **/
export class TestRunnerResultsEvent extends TypedEvent {
    results;
    /**
     * Creates a new test result event containing the results.
     */
    constructor(results) {
        super("testrunnerresults");
        this.results = results;
    }
}
