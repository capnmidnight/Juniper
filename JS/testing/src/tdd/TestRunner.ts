import { PriorityMap } from "@juniper-lib/collections/dist/PriorityMap";
import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { isFunction, isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
import type { TestCase, TestCaseConstructor } from "./TestCase";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestResults, TestRunnerResultsEvent } from "./TestRunnerResultsEvent";
import { TestScore } from "./TestScore";
import { makeErrorMessage } from "../../../tslib/src/makeErrorMessage";

function testNames(TestClass: TestCase): (keyof TestCase)[] {
    const names = Object.getOwnPropertyNames(TestClass);
    names.sort();
    return names as (keyof TestCase)[];
}

function isTest(testCase: TestCase, name: keyof TestCase) {
    return name.startsWith("test_")
        && isFunction(testCase[name]);
}

type TestRunnerEvents = {
    testrunnerresults: TestRunnerResultsEvent;
}

export class TestRunner extends TypedEventTarget<TestRunnerEvents> {

    private readonly props: any[];
    private readonly CaseClasses: TestCaseConstructor[];

    constructor(...rest: TestCaseConstructor[]) {
        super();
        this.props = rest.filter((v) => !isFunction(v));
        this.CaseClasses = rest.filter((v) => isFunction(v));
    }

    scaffold() {
        const results: TestResults = new PriorityMap();
        for (const CaseClass of this.CaseClasses) {
            for (const name of testNames(CaseClass.prototype)) {
                if (isTest(CaseClass.prototype, name)) {
                    results.add(CaseClass.name, name, new TestScore(name));
                }
            }
        }
        this.dispatchEvent(new TestRunnerResultsEvent(results));
    }

    async run(testCaseName: string, testName: string) {
        const onUpdate = () => this.dispatchEvent(new TestRunnerResultsEvent(results));

        const results: TestResults = new PriorityMap();
        const q = new Array<() => Promise<void>>();
        for (const CaseClass of this.CaseClasses) {
            for (const funcName of testNames(CaseClass.prototype)) {
                if (isTest(CaseClass.prototype, funcName)) {
                    results.add(CaseClass.name, funcName, new TestScore(funcName));

                    if ((isNullOrUndefined(testCaseName) || CaseClass.name === testCaseName)
                        && (isNullOrUndefined(testName) || funcName === testName)) {
                        q.push(() => this.runTest(CaseClass, funcName, results, CaseClass.name, onUpdate));
                    }
                }
            }
        }

        const update = async () => {
            onUpdate();
            const N = 10;
            for (let i = 0; i < N && q.length > 0; ++i) {
                const test = q.shift();
                await test();
                if (i === N - 1) {
                    setTimeout(update);
                }
            }
        };
        update();
    }

    private async runTest(CaseClass: TestCaseConstructor, funcName: string, results: TestResults, className: string, onUpdate: Function) {
        const testCase = new CaseClass(),
            func = (testCase as any)[funcName],
            caseResults = results.get(className),
            score = caseResults.get(funcName);

        const onMessage = (evt: TestCaseMessageEvent) => {
            score.messages.push(evt.message);
            onUpdate();
        };

        const onSuccess = () => {
            score.success();
            onUpdate();
        };

        const onFailure = (evt: TestCaseFailEvent) => {
            score.fail(evt.message);
            onUpdate();
        };

        for (const prop of this.props) {
            Object.assign(testCase, prop);
        }

        testCase.addEventListener("testcasemessage", onMessage);
        testCase.addEventListener("testcasesuccess", onSuccess);
        testCase.addEventListener("testcasefail", onFailure);

        let message = null;
        try {
            score.start();
            onUpdate();
            testCase.setup();
            message = await func.call(testCase);
        }
        catch (exp) {
            console.error(`Test case failed [${className}::${funcName}]`, exp);
            message = exp;
            onFailure(new TestCaseFailEvent(makeErrorMessage(exp)));
        }
        score.finish(message);
        onUpdate();
        testCase.teardown();
        testCase.clearEventListeners();
        onUpdate();
    }
}
