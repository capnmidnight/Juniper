import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
import { TestResults } from "./TestRunnerResultsEvent";
export interface TestStats {
    totalFound: number;
    totalRan: number;
    totalCompleted: number;
    totalIncomplete: number;
    totalSucceeded: number;
    totalFailed: number;
}
export declare class TestOutputResultsEvent extends TypedEvent<"testoutputresults"> {
    readonly results: TestResults;
    readonly stats: TestStats;
    constructor(results: TestResults, stats: TestStats);
}
//# sourceMappingURL=TestOutputResultsEvent.d.ts.map