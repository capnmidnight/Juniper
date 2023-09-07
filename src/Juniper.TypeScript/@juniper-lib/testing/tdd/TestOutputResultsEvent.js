import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
export class TestOutputResultsEvent extends TypedEvent {
    constructor(results, stats) {
        super("testoutputresults");
        this.results = results;
        this.stats = stats;
    }
}
//# sourceMappingURL=TestOutputResultsEvent.js.map