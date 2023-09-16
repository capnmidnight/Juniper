import { ID } from "@juniper-lib/dom/attrs";
import { backgroundColor, color, columnGap, display, em, fr, getMonospaceFamily, gridColumn, gridTemplateColumns, height, overflow, perc, whiteSpace, width } from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import { Button, Div, Span, elementReplace } from "@juniper-lib/dom/tags";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { TestOutput } from "./TestOutput";
import { TestStates } from "./TestStates";
/**
 * Creates a portion of a progress bar.
 */
function bar(c, w) {
    return [
        backgroundColor(c),
        color(c),
        width(`${w}%`)
    ];
}
function refresher(thunk, ...rest) {
    return Button(onClick(thunk), gridColumn(1), "\u{1F504}\u{FE0F}", ...rest);
}
function makeStatus(id) {
    const complete = id & TestStates.completed;
    if (id & TestStates.failed) {
        return complete ? "Failure" : "Failing";
    }
    else if (id & TestStates.succeeded) {
        return complete ? "Success" : "Succeeding";
    }
    else if (id & TestStates.started) {
        return complete ? "No test ran" : "Running";
    }
    else {
        return "Found";
    }
}
export class TestOutputHTML extends TestOutput {
    constructor(...CaseClasses) {
        super(...CaseClasses);
        this.element = Div(ID("testOutput"));
        let lastTable = null;
        this.addEventListener("testoutputresults", (evt) => {
            const s = Math.round(100 * evt.stats.totalSucceeded / evt.stats.totalFound), f = Math.round(100 * evt.stats.totalFailed / evt.stats.totalFound), t = Math.round(100 * (evt.stats.totalFound - evt.stats.totalSucceeded - evt.stats.totalFailed) / evt.stats.totalFound), basicStyle = [
                display("inline-block"),
                overflow("hidden"),
                height(em(1))
            ], table = Div(display("grid"), gridTemplateColumns("auto", "auto", "auto", "auto", fr(1)), getMonospaceFamily(), width(perc(100)), columnGap(em(1)), refresher(() => this.run()), Div(gridColumn(2, -1), height(em(2)), whiteSpace("nowrap"), overflow("hidden"), Span(...basicStyle, ...bar("green", s)), Span(...basicStyle, ...bar("red", f)), Span(...basicStyle, ...bar("grey", t))), Div(gridColumn(1), "Rerun"), Div(gridColumn(2, 4), "Name"), Div(gridColumn(4, -1), "Status"));
            let lastTestCaseName = null;
            for (const [testCaseName, testName, test] of evt.results.entries()) {
                if (testCaseName !== lastTestCaseName) {
                    lastTestCaseName = testCaseName;
                    table.append(refresher(() => this.run(testCaseName)), Div(gridColumn(2, -1), testCaseName));
                }
                table.append(refresher(() => this.run(testCaseName, testName)), Div(gridColumn(3), testName), Div(gridColumn(4), makeStatus(test.state)), Div(gridColumn(5), test.messages.join(", ")));
            }
            if (isDefined(lastTable)) {
                elementReplace(lastTable, table);
            }
            else {
                this.element.append(table);
            }
            lastTable = table;
        });
        this.scaffold();
    }
}
//# sourceMappingURL=TestOutputHTML.js.map