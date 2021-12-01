import { backgroundColor, Button, col, color, columnGap, display, Div, elementClearChildren, getMonospaceFamily, gridColsDef, height, id, onClick, overflow, Span, styles, whiteSpace, width } from "juniper-dom";
import { TestOutput } from "./TestOutput";
import { TestStates } from "./TestStates";
/**
 * Creates a portion of a progress bar.
 */
function bar(c, w) {
    return styles(backgroundColor(c), color(c), width(`${w}%`));
}
function refresher(thunk, ...rest) {
    return Button(onClick(thunk), col(1), "\u{1F504}\u{FE0F}", ...rest);
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
export class HtmlTestOutput extends TestOutput {
    element;
    constructor(...CaseClasses) {
        super(...CaseClasses);
        this.element = Div(id("testOutput"));
        const draw = (evt) => {
            const s = Math.round(100 * evt.stats.totalSucceeded / evt.stats.totalFound), f = Math.round(100 * evt.stats.totalFailed / evt.stats.totalFound), t = Math.round(100 * (evt.stats.totalFound - evt.stats.totalSucceeded - evt.stats.totalFailed) / evt.stats.totalFound), basicStyle = styles(display("inline-block"), overflow("hidden"), height("1em")), table = Div(gridColsDef("auto", "auto", "auto", "1fr"), getMonospaceFamily(), width("100%"), columnGap("1em"), refresher(() => this.run()), Div(col(2, 3), height("2em"), whiteSpace("nowrap"), overflow("hidden"), Span(basicStyle, bar("green", s)), Span(basicStyle, bar("red", f)), Span(basicStyle, bar("grey", t))), Div(col(1), "Rerun"), Div(col(2), "Name"), Div(col(3), "Status"));
            for (let [testCaseName, testCase] of evt.results.entries()) {
                table.append(Div(col(2, 3), testCaseName), refresher(() => this.run(testCaseName)));
                for (let [testName, test] of testCase.entries()) {
                    table.append(refresher(() => this.run(testCaseName, testName)), Div(col(2), testName), Div(col(3), makeStatus(test.state)), Div(col(4), test.messages.join(", ")));
                }
            }
            elementClearChildren(this.element);
            this.element.append(table);
        };
        this.addEventListener("testoutputresults", draw);
    }
}
