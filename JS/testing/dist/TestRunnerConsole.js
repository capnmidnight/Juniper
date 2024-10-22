import { BaseTestRunner } from "./BaseTestRunner";
export class TestRunnerConsole extends BaseTestRunner {
    log(...msg) {
        console.log(...msg);
    }
    success(...msg) {
        console.log(...msg);
    }
    fail(...msg) {
        console.warn(...msg);
    }
    error(exp, ...msg) {
        console.error(...msg, exp);
    }
}
//# sourceMappingURL=TestRunnerConsole.js.map