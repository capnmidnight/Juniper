import { PriorityMap } from "@juniper-lib/collections/PriorityMap";
import { TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { isFunction, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestRunnerResultsEvent } from "./TestRunnerResultsEvent";
import { TestScore } from "./TestScore";
function testNames(TestClass) {
    const names = Object.getOwnPropertyNames(TestClass);
    names.sort();
    return names;
}
function isTest(testCase, name) {
    return name.startsWith("test_")
        && isFunction(testCase[name]);
}
export class TestRunner extends TypedEventTarget {
    constructor(...rest) {
        super();
        this.props = rest.filter((v) => !isFunction(v));
        this.CaseClasses = rest.filter((v) => isFunction(v));
    }
    scaffold() {
        const results = new PriorityMap();
        for (const CaseClass of this.CaseClasses) {
            for (const name of testNames(CaseClass.prototype)) {
                if (isTest(CaseClass.prototype, name)) {
                    results.add(CaseClass.name, name, new TestScore(name));
                }
            }
        }
        this.dispatchEvent(new TestRunnerResultsEvent(results));
    }
    async run(testCaseName, testName) {
        const onUpdate = () => this.dispatchEvent(new TestRunnerResultsEvent(results));
        const results = new PriorityMap();
        const q = new Array();
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
    async runTest(CaseClass, funcName, results, className, onUpdate) {
        const testCase = new CaseClass(), func = testCase[funcName], caseResults = results.get(className), score = caseResults.get(funcName);
        const onMessage = (evt) => {
            score.messages.push(evt.message);
            onUpdate();
        };
        const onSuccess = () => {
            score.success();
            onUpdate();
        };
        const onFailure = (evt) => {
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
            onFailure(new TestCaseFailEvent(exp));
        }
        score.finish(message);
        onUpdate();
        testCase.teardown();
        testCase.clearEventListeners();
        onUpdate();
    }
}
//# sourceMappingURL=TestRunner.js.map