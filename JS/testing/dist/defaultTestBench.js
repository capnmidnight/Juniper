import { TestRunnerConsole } from "./TestRunnerConsole";
import { TestRunnerHTML } from "./TestRunnerHTML";
const testRunner = "document" in globalThis
    ? new TestRunnerHTML()
    : new TestRunnerConsole();
console.log(testRunner);
export function test(name, args, ...callbacks) {
    testRunner.test(name, args, ...callbacks);
}
export function log(...msg) {
    testRunner.log(...msg);
}
//# sourceMappingURL=defaultTestBench.js.map