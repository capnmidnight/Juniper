import { ErsatzElement } from "juniper-dom";
import { CaseClassConstructor } from "./CaseClassConstructor";
import { TestOutput } from "./TestOutput";
export declare class HtmlTestOutput extends TestOutput implements ErsatzElement {
    readonly element: HTMLElement;
    constructor(...CaseClasses: CaseClassConstructor[]);
}
