import { BaseTestRunner } from "./BaseTestRunner";
export declare class TestRunnerConsole extends BaseTestRunner {
    log(...msg: any[]): void;
    success(...msg: any[]): void;
    fail(...msg: any[]): void;
    error(exp: Error, ...msg: any[]): void;
}
//# sourceMappingURL=TestRunnerConsole.d.ts.map