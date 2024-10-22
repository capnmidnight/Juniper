import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { isNumber, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestCaseSuccessEvent } from "./TestCaseSuccessEvent";
export class TestCase extends TypedEventTarget {
    constructor() {
        super(...arguments);
        this.defaultError = 0.001;
    }
    setup() { }
    teardown() { }
    message(msg) {
        msg = msg || "N/A";
        this.dispatchEvent(new TestCaseMessageEvent(msg));
    }
    success() {
        this.dispatchEvent(new TestCaseSuccessEvent());
        return true;
    }
    fail(msg) {
        msg = msg || "Fail";
        this.dispatchEvent(new TestCaseFailEvent(msg));
        return false;
    }
    areSame(actual, expected, message) {
        return this.twoValueTest(actual, "==", expected, (a, b) => a == b, message);
    }
    areExact(actual, expected, message) {
        return this.twoValueTest(actual, "===", expected, (a, b) => a === b, message);
    }
    arraysMatch(actual, expected, message) {
        return this.twoValueTest(actual, "==", expected, (a, b) => a.length === b.length
            && a.map((v, i) => v === b[i])
                .reduce((a, b) => a && b, true), message);
    }
    areApprox(actual, expected, messageOrExpectedError, expectedError) {
        expectedError = isNumber(expectedError) && expectedError
            || isNumber(messageOrExpectedError) && messageOrExpectedError
            || this.defaultError;
        const actualError = Math.abs(actual - expected);
        const pre = isString(messageOrExpectedError) && messageOrExpectedError
            || "";
        const message = `(${actualError}) ${pre}`;
        return this.twoValueTest(actual, "~==", expected, (a, b) => Math.abs(a - b) <= expectedError, message);
    }
    isNull(value, message) {
        return this.areExact(value, null, message);
    }
    isNotNull(value, message) {
        return this.isNotEqualTo(value, null, message);
    }
    isUndefined(value, message) {
        return this.areExact(value, undefined, message);
    }
    isNotUndefined(value, message) {
        return this.isNotEqualTo(value, undefined, message);
    }
    isTrue(value, message) {
        return this.areExact(value, true, message);
    }
    isTruthy(value, message) {
        return this.isTrue(!!value, message);
    }
    isFalse(value, message) {
        return this.areExact(value, false, message);
    }
    isFalsey(value, message) {
        return this.isFalse(!!value, message);
    }
    isBoolean(value, message) {
        return this.areExact(value === true || value === false, true, message);
    }
    hasValue(value, message) {
        message = message || value;
        const goodMessage = `${message} is a value`, badMessage = `${message} is not a value`, isValue = value !== null
            && value !== undefined;
        return this.isTrue(isValue, isValue ? goodMessage : badMessage);
    }
    isEmpty(value, message) {
        message = message || `${value} is empty`;
        return this.areExact(value.length, 0, message);
    }
    isNotEqualTo(actual, expected, message) {
        return this.twoValueTest(actual, "!==", expected, (a, b) => a !== b, message);
    }
    areDifferent(actual, expected, message) {
        return this.twoValueTest(actual, "!=", expected, (a, b) => a != b, message);
    }
    isLessThan(actual, expected, message) {
        return this.twoValueTest(actual, "<", expected, (a, b) => a < b, message);
    }
    isLessThanEqual(actual, expected, message) {
        return this.twoValueTest(actual, "<=", expected, (a, b) => a <= b, message);
    }
    isGreaterThan(actual, expected, message) {
        return this.twoValueTest(actual, ">", expected, (a, b) => a > b, message);
    }
    isGreaterThanEqual(actual, expected, message) {
        return this.twoValueTest(actual, ">=", expected, (a, b) => a >= b, message);
    }
    throws(func, message) {
        return this.throwTest(func, true, message);
    }
    doesNotThrow(func, message) {
        return this.throwTest(func, false, message);
    }
    async resolves(task) {
        await this.doesNotThrow(() => task);
    }
    async rejects(task) {
        await this.throws(() => task);
    }
    twoValueTest(actual, op, expected, testFunc, message) {
        if (testFunc(actual, expected)) {
            return this.success();
        }
        else {
            return this.fail(`${message || ""} [Actual: ${actual}] ${op} [Expected: ${expected}]`);
        }
    }
    async throwTest(func, op, message) {
        let threw = false;
        try {
            await func();
        }
        catch (exp) {
            if (!op) {
                console.error(exp);
            }
            threw = true;
        }
        const testValue = threw === op, testString = testValue ? "Success!" : "Fail!", throwMessage = op ? "throw" : "not throw", testMessage = `Expected function to ${throwMessage} -> ${testString}`;
        message = ((message && message + ". ") || "") + testMessage;
        if (testValue) {
            return this.success();
        }
        else {
            return this.fail(message);
        }
    }
}
//# sourceMappingURL=TestCase.js.map