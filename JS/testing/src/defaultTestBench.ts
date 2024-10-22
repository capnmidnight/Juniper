

import { TestRunnerConsole } from "./TestRunnerConsole";
import { TestRunnerHTML } from "./TestRunnerHTML";

const testRunner = "document" in globalThis
    ? new TestRunnerHTML()
    : new TestRunnerConsole();

console.log(testRunner);

export function test(name: string, args: string, ...callbacks: VoidFunction[]) {
    testRunner.test(name, args, ...callbacks);
}

export function log(...msg: any[]) {
    testRunner.log(...msg);
}