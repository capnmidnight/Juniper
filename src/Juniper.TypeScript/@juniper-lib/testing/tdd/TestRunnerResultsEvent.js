import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
/**
 * An Event that encapsulates a test result.
 **/
export class TestRunnerResultsEvent extends TypedEvent {
    /**
     * Creates a new test result event containing the results.
     */
    constructor(results) {
        super("testrunnerresults");
        this.results = results;
    }
}
//# sourceMappingURL=TestRunnerResultsEvent.js.map