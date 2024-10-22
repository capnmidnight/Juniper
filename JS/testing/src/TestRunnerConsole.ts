import { BaseTestRunner } from "./BaseTestRunner";

export class TestRunnerConsole extends BaseTestRunner {
    
    override log(...msg: any[]) {
        console.log(...msg);
    }

    override success(...msg: any[]) {
        console.log(...msg);
    }

    override fail(...msg: any[]) {
        console.warn(...msg);
    }

    override error(exp: Error, ...msg: any[]) {
        console.error(...msg, exp);
    }
}
