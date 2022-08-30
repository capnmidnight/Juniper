import { id } from "@juniper-lib/dom/attrs";
import {
    backgroundColor,
    color,
    columnGap,
    display,
    getMonospaceFamily,
    gridColumn,
    gridTemplateColumns,
    height,
    overflow,
    paddingTop,
    px,
    whiteSpace,
    width
} from "@juniper-lib/dom/css";
import { onClick } from "@juniper-lib/dom/evts";
import {
    Button,
    Div,
    ElementChild, elementReplace, ErsatzElement,
    Span
} from "@juniper-lib/dom/tags";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { TestCaseConstructor } from "./TestCase";
import { TestOutput } from "./TestOutput";
import { TestOutputResultsEvent } from "./TestOutputResultsEvent";
import { TestStates } from "./TestStates";


/**
 * Creates a portion of a progress bar.
 */
function bar(c: CSSColorValue, w: number) {
    return [
        backgroundColor(c),
        color(c),
        width(`${w}%`)
    ];
}

function refresher(thunk: (evt: Event) => void, ...rest: ElementChild[]) {
    return Button(
        onClick(thunk),
        gridColumn(1),
        "\u{1F504}\u{FE0F}",
        ...rest);
}

function makeStatus(id: TestStates) {
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

export class TestOutputHTML extends TestOutput implements ErsatzElement {

    readonly element: HTMLElement;

    constructor(...CaseClasses: TestCaseConstructor[]) {
        super(...CaseClasses);
        this.element = Div(id("testOutput"));
        let lastTable: HTMLDivElement = null;
        this.addEventListener("testoutputresults", (evt: TestOutputResultsEvent) => {
            const s = Math.round(100 * evt.stats.totalSucceeded / evt.stats.totalFound),
                f = Math.round(100 * evt.stats.totalFailed / evt.stats.totalFound),
                t = Math.round(100 * (evt.stats.totalFound - evt.stats.totalSucceeded - evt.stats.totalFailed) / evt.stats.totalFound),
                basicStyle = [
                    display("inline-block"),
                    overflow("hidden"),
                    height("1em")
                ],
                table = Div(
                    display("grid"),
                    gridTemplateColumns("auto", "auto", "auto", "1fr"),
                    getMonospaceFamily(),
                    width("100%"),
                    columnGap("1em"),
                    refresher(() => this.run()),
                    Div(
                        gridColumn(2, 5),
                        height("2em"),
                        whiteSpace("nowrap"),
                        overflow("hidden"),
                        Span(...basicStyle, ...bar("green", s)),
                        Span(...basicStyle, ...bar("red", f)),
                        Span(...basicStyle, ...bar("grey", t))),
                    Div(gridColumn(1), "Rerun"),
                    Div(gridColumn(2), "Name"),
                    Div(gridColumn(3), "Status"));

            let lastTestCaseName: string = null;
            for (let [testCaseName, testName, test] of evt.results.entries()) {
                if (testCaseName !== lastTestCaseName) {
                    lastTestCaseName = testCaseName;
                    table.append(
                        refresher(() => this.run(testCaseName)),
                        Div(
                            gridColumn(2, 5),
                            paddingTop(px(20)),
                            testCaseName));
                }
                table.append(
                    refresher(() => this.run(testCaseName, testName)),
                    Div(gridColumn(2), testName),
                    Div(gridColumn(3), makeStatus(test.state)),
                    Div(gridColumn(4), test.messages.join(", ")));
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
