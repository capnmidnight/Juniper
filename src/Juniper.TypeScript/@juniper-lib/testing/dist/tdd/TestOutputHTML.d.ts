import { ErsatzElement } from "@juniper-lib/dom/dist/tags";
import { TestCaseConstructor } from "./TestCase";
import { TestOutput } from "./TestOutput";
export declare class TestOutputHTML extends TestOutput implements ErsatzElement {
    readonly element: HTMLElement;
    constructor(...CaseClasses: TestCaseConstructor[]);
}
//# sourceMappingURL=TestOutputHTML.d.ts.map