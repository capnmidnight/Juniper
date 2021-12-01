import { CaseClassConstructor } from "./CaseClassConstructor";
import { TestOutput } from "./TestOutput";
/**
 * A test outputer that runs in the console.
 **/
export declare class ConsoleTestOutput extends TestOutput {
    /**
     * Creates a new test outputer that runs in the console.
     */
    constructor(...CaseClasses: CaseClassConstructor[]);
}
