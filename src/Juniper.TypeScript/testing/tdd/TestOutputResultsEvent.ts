import { TypedEvent } from "@juniper/events";
import { TestResults } from "./TestRunnerResultsEvent";

export interface TestStats {
    totalFound: number;
    totalRan: number;
    totalCompleted: number;
    totalIncomplete: number;
    totalSucceeded: number;
    totalFailed: number;
}

export class TestOutputResultsEvent extends TypedEvent<"testoutputresults"> {
    constructor(public readonly results: TestResults, public readonly stats: TestStats) {
        super("testoutputresults");
    }
}
