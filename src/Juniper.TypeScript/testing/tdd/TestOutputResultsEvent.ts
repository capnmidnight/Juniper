import { TypedEvent } from "juniper-tslib";

export interface TestStats {
    totalFound: number;
    totalRan: number;
    totalCompleted: number;
    totalIncomplete: number;
    totalSucceeded: number;
    totalFailed: number;
}

export class TestOutputResultsEvent extends TypedEvent<"testoutputresults"> {
    /**
     * 
     * @param {any} results
     * @param {TestStats} stats
     */
    constructor(public readonly results: any, public readonly stats: TestStats) {
        super("testoutputresults");
    }
}
