import { TypedEvent } from "juniper-tslib";
export class TestOutputResultsEvent extends TypedEvent {
    results;
    stats;
    /**
     *
     * @param {any} results
     * @param {TestStats} stats
     */
    constructor(results, stats) {
        super("testoutputresults");
        this.results = results;
        this.stats = stats;
    }
}
