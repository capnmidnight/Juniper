import { TestCaseConstructor } from "./TestCase";
import { TestOutput } from "./TestOutput";
import { TestOutputResultsEvent } from "./TestOutputResultsEvent";

/**
 * A test outputer that runs in the console.
 **/
export class TestOutputConsole extends TestOutput {
    /**
     * Creates a new test outputer that runs in the console.
     */
    constructor(...CaseClasses: TestCaseConstructor[]) {
        super(...CaseClasses);

        const onResults = (evt: TestOutputResultsEvent) => {
            console.clear();
            for (const testCaseName of evt.results.keys()) {
                console.group(testCaseName);
                console.table(evt.results.get(testCaseName));
                console.groupEnd();
            }
            console.table(evt.stats);
            let foundLabel = "%c", failedLabel = "%c", succeededLabel = "%c";
            while (evt.stats.totalFound > 0) {
                if (evt.stats.totalFailed > 0) {
                    failedLabel += "o";
                    --evt.stats.totalFailed;
                }
                else if (evt.stats.totalSucceeded > 0) {
                    succeededLabel += "o";
                    --evt.stats.totalSucceeded;
                }
                else {
                    foundLabel += "o";
                }
                --evt.stats.totalFound;
            }
            console.log(succeededLabel + failedLabel + foundLabel, "color: green", "color: red", "color: grey");
        };

        this.addEventListener("testoutputresults", onResults);
    }
}
