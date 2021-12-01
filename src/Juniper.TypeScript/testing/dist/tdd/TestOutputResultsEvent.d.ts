import { TypedEvent } from "juniper-tslib";
export interface TestStats {
    totalFound: number;
    totalRan: number;
    totalCompleted: number;
    totalIncomplete: number;
    totalSucceeded: number;
    totalFailed: number;
}
export declare class TestOutputResultsEvent extends TypedEvent<"testoutputresults"> {
    readonly results: any;
    readonly stats: TestStats;
    /**
     *
     * @param {any} results
     * @param {TestStats} stats
     */
    constructor(results: any, stats: TestStats);
}
