import { isNumber, isString, TypedEventBase } from "juniper-tslib";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestCaseSuccessEvent } from "./TestCaseSuccessEvent";

interface TestCaseEvents {
    testcasemessage: TestCaseMessageEvent;
    testcasesuccess: TestCaseSuccessEvent;
    testcasefail: TestCaseFailEvent;
}

export class TestCase extends TypedEventBase<TestCaseEvents> {

    protected defaultError = 0.001;

    setup() { }

    teardown() { }

    message(msg: string) {
        msg = msg || "N/A";
        this.dispatchEvent(new TestCaseMessageEvent(msg));
    }

    success() {
        this.dispatchEvent(new TestCaseSuccessEvent());
    }

    fail(msg: string) {
        msg = msg || "Fail";
        this.dispatchEvent(new TestCaseFailEvent(msg));
    }

    areSame(actual: any, expected: any, message?: string) {
        this.twoValueTest(actual, "==", expected, (a, b) => a == b, message);
    }

    areExact<T>(actual: T, expected: T, message?: string) {
        this.twoValueTest(actual, "===", expected, (a, b) => a === b, message);
    }

    areApprox(actual: number, expected: number, error?: number): void;
    areApprox(actual: number, expected: number, message: string, error?: number): void;
    areApprox(actual: number, expected: number, messageOrError?: (string | number), maybeError?: number): void {
        const error = isNumber(maybeError) && maybeError
            || isNumber(messageOrError) && messageOrError
            || this.defaultError;
        const message = isString(messageOrError) && messageOrError
            || null;

        this.twoValueTest(actual, "~==", expected, (a, b) => Math.abs(b - a) <= error, message);
    }

    isNull(value: any, message?: string) {
        this.areExact(value, null, message);
    }

    isNotNull(value: any, message?: string) {
        this.isNotEqualTo(value, null, message);
    }

    isUndefined(value: any, message?: string) {
        this.areExact(value, undefined, message);
    }

    isNotUndefined(value: any, message?: string) {
        this.isNotEqualTo(value, undefined, message);
    }

    isTrue(value: boolean, message?: string) {
        this.areExact(value, true, message);
    }

    isTruthy(value: any, message?: string) {
        this.isTrue(!!value, message);
    }

    isFalse(value: boolean, message?: string) {
        this.areExact(value, false, message);
    }

    isFalsey(value: any, message?: string) {
        this.isFalse(!!value, message);
    }

    isBoolean(value: unknown, message?: string) {
        this.areExact(value === true || value === false, true, message);
    }

    hasValue(value: any, message?: string) {
        message = message || value;
        const goodMessage = `${message} is a value`,
            badMessage = `${message} is not a value`,
            isValue = value !== null
                && value !== undefined;
        this.isTrue(isValue, isValue ? goodMessage : badMessage);
    }

    isEmpty(value: string, message?: string) {
        message = message || `${value} is empty`;
        this.areExact(value.length, 0, message);
    }

    isNotEqualTo<T>(actual: T, expected: T, message?: string) {
        this.twoValueTest(actual, "!==", expected, (a, b) => a !== b, message);
    }

    isLessThan<T>(actual: T, expected: T, message?: string) {
        this.twoValueTest(actual, "<", expected, (a, b) => a < b, message);
    }

    isLessThanEqual<T>(actual: T, expected: T, message?: string) {
        this.twoValueTest(actual, "<=", expected, (a, b) => a <= b, message);
    }

    isGreaterThan<T>(actual: T, expected: T, message?: string) {
        this.twoValueTest(actual, ">", expected, (a, b) => a > b, message);
    }

    isGreaterThanEqual<T>(actual: T, expected: T, message?: string) {
        this.twoValueTest(actual, ">=", expected, (a, b) => a >= b, message);
    }

    async throws(func: Function, message?: string) {
        await this.throwTest(func, true, message);
    }

    async doesNotThrow(func: Function, message?: string) {
        await this.throwTest(func, false, message);
    }

    private twoValueTest<T>(actual: T, op: string, expected: T, testFunc: (a: T, b: T) => boolean, message?: string) {
        if (testFunc(actual, expected)) {
            this.success();
        }
        else {
            this.fail(`${message || ""} [Actual: ${actual}] ${op} [Expected: ${expected}]`);
        }
    }

    private async throwTest(func: Function, op: boolean, message?: string) {
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
        const testValue = threw === op,
            testString = testValue ? "Success!" : "Fail!",
            throwMessage = op ? "throw" : "not throw",
            testMessage = `Expected function to ${throwMessage} -> ${testString}`;
        message = ((message && message + ". ") || "") + testMessage;
        if (testValue) {
            this.success();
        }
        else {
            this.fail(message);
        }
    }
}
