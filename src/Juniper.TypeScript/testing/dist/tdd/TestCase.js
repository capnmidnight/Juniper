import { TypedEventBase } from "juniper-tslib";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestCaseSuccessEvent } from "./TestCaseSuccessEvent";
export class TestCase extends TypedEventBase {
    setup() { }
    teardown() { }
    message(msg) {
        msg = msg || "N/A";
        this.dispatchEvent(new TestCaseMessageEvent(msg));
    }
    success() {
        this.dispatchEvent(new TestCaseSuccessEvent());
    }
    fail(msg) {
        msg = msg || "Fail";
        this.dispatchEvent(new TestCaseFailEvent(msg));
    }
    isEqualTo(actual, expected, message) {
        this.twoValueTest(actual, "===", expected, (a, b) => a === b, message);
    }
    isNull(value, message) {
        this.isEqualTo(value, null, message);
    }
    isNotNull(value, message) {
        this.isNotEqualTo(value, null, message);
    }
    isUndefined(value, message) {
        this.isEqualTo(value, undefined, message);
    }
    isNotUndefined(value, message) {
        this.isNotEqualTo(value, undefined, message);
    }
    isTrue(value, message) {
        this.isEqualTo(value, true, message);
    }
    isFalse(value, message) {
        this.isEqualTo(value, false, message);
    }
    isBoolean(value, message) {
        this.isEqualTo(value === true || value === false, true, message);
    }
    hasValue(value, message) {
        message = message || value;
        const goodMessage = `${message} is a value`, badMessage = `${message} is not a value`, isValue = value !== null
            && value !== undefined;
        this.isTrue(isValue, isValue ? goodMessage : badMessage);
    }
    isEmpty(value, message) {
        message = message || `${value} is empty`;
        this.isEqualTo(value.length, 0, message);
    }
    isNotEqualTo(actual, expected, message) {
        this.twoValueTest(actual, "!==", expected, (a, b) => a !== b, message);
    }
    isLessThan(actual, expected, message) {
        this.twoValueTest(actual, "<", expected, (a, b) => a < b, message);
    }
    isLessThanEqual(actual, expected, message) {
        this.twoValueTest(actual, "<=", expected, (a, b) => a <= b, message);
    }
    isGreaterThan(actual, expected, message) {
        this.twoValueTest(actual, ">", expected, (a, b) => a > b, message);
    }
    isGreaterThanEqual(actual, expected, message) {
        this.twoValueTest(actual, ">=", expected, (a, b) => a >= b, message);
    }
    throws(func, message) {
        this.throwTest(func, true, message);
    }
    doesNotThrow(func, message) {
        this.throwTest(func, false, message);
    }
    twoValueTest(actual, op, expected, testFunc, message) {
        if (testFunc(actual, expected)) {
            this.success();
        }
        else {
            this.fail(`${message || ""} [Actual: ${actual}] ${op} [Expected: ${expected}]`);
        }
    }
    throwTest(func, op, message) {
        let threw = false;
        try {
            func();
        }
        catch (exp) {
            threw = true;
        }
        const testValue = threw === op, testString = testValue ? "Success!" : "Fail!", throwMessage = op ? "throw" : "not throw", testMessage = `Expected function to ${throwMessage} -> ${testString}`;
        message = ((message && message + ". ") || "") + testMessage;
        if (testValue) {
            this.success();
        }
        else {
            this.fail(message);
        }
    }
}
