import { isFunction, TypedEventBase } from "juniper-tslib";
import { CaseClassConstructor } from "./CaseClassConstructor";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestResults, TestRunnerResultsEvent } from "./TestRunnerResultsEvent";
import { TestScore } from "./TestScore";

function testNames(TestClass: Function) {
    const names = Object.getOwnPropertyNames(TestClass);
    names.sort();
    return names;
}

function isTest(testCase: any, name: string, testName?: string) {
    return (name === testName
        || ((testName === undefined || testName === null)
            && name.startsWith("test_")))
        && testCase[name] instanceof Function;
}

interface TestRunnerEvents {
    testrunnerresults: TestRunnerResultsEvent;
}

export class TestRunner extends TypedEventBase<TestRunnerEvents> {

    private readonly props: any[];
    private readonly CaseClasses: CaseClassConstructor[];

    constructor(...rest: any[]) {
        super();
        this.props = rest.filter(v => !isFunction(v));
        this.CaseClasses = rest.filter(v => isFunction(v));
    }

    async run(testCaseName: string, testName: string) {
        const results: TestResults = new Map();
        const onUpdate = () => this.dispatchEvent(new TestRunnerResultsEvent(results));
        for (let CaseClass of this.CaseClasses) {
            const caseResults = new Map();
            results.set(CaseClass.name, caseResults);
            for (let name of testNames(CaseClass.prototype)) {
                if (isTest(CaseClass.prototype, name)) {
                    caseResults.set(name, new TestScore(name));
                    onUpdate();
                }
            }
        }

        /**
        * @callback testRunCallback
        * @returns {Promise}
        */

        /** @type {testRunCallback[]} */
        const q: Array<() => Promise<any>> = [];
        for (let CaseClass of this.CaseClasses) {
            const className = CaseClass.name;
            if (className === testCaseName
                || testCaseName === undefined
                || testCaseName === null) {
                for (let funcName of testNames(CaseClass.prototype)) {
                    if (isTest(CaseClass.prototype, funcName, testName)) {
                        q.push(() => this.runTest(CaseClass, funcName, results, className, onUpdate));
                    }
                }
            }
        }

        const restart = () => setTimeout(update, 0),
            update = () => {
                if (q.length > 0) {
                    const test = q.shift();
                    const run = test();
                    run.then(restart)
                        .catch(restart);
                }
            };
        restart();
    }

    async runTest(CaseClass: CaseClassConstructor, funcName: string, results: TestResults, className: string, onUpdate: Function) {
        const testCase = new CaseClass(),
            func = testCase[funcName],
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

        for (let prop of this.props) {
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
            message = func.call(testCase);
            if (message instanceof Promise) {
                message = await message;
            }
        }
        catch (exp) {
            message = exp;
            onFailure(new TestCaseFailEvent(exp));
        }
        score.finish(message);
        onUpdate();
        testCase.teardown();
        testCase.removeEventListener("testcasefail", onFailure);
        testCase.removeEventListener("testcasesuccess", onSuccess);
        testCase.removeEventListener("testcasemessage", onMessage);
        onUpdate();
    }
}
