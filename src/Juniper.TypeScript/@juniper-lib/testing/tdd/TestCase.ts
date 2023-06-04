import { TypedEventBase } from "@juniper-lib/events/EventBase";
import { isNumber, isString } from "@juniper-lib/tslib/typeChecks";
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

    success(): true {
        this.dispatchEvent(new TestCaseSuccessEvent());
        return true;
    }

    fail(msg: string): false {
        msg = msg || "Fail";
        this.dispatchEvent(new TestCaseFailEvent(msg));
        return false;
    }

    areSame(actual: any, expected: any, message?: string): boolean {
        return this.twoValueTest(actual, "==", expected, (a, b) => a == b, message);
    }

    areExact<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, "===", expected, (a, b) => a === b, message);
    }

    areApprox(actual: number, expected: number, expectedError?: number): boolean;
    areApprox(actual: number, expected: number, message: string, expectedError?: number): boolean;
    areApprox(actual: number, expected: number, messageOrExpectedError?: (string | number), expectedError?: number): boolean {
        expectedError = isNumber(expectedError) && expectedError
            || isNumber(messageOrExpectedError) && messageOrExpectedError
            || this.defaultError;

        const actualError = Math.abs(actual - expected);

        const pre = isString(messageOrExpectedError) && messageOrExpectedError
            || "";

        const message = `(${actualError}) ${pre}`;

        return this.twoValueTest(actual, "~==", expected, (a, b) => Math.abs(a - b) <= expectedError, message);
    }

    isNull(value: any, message?: string): boolean {
        return this.areExact(value, null, message);
    }

    isNotNull(value: any, message?: string): boolean {
        return this.isNotEqualTo(value, null, message);
    }

    isUndefined(value: any, message?: string): boolean {
        return this.areExact(value, undefined, message);
    }

    isNotUndefined(value: any, message?: string): boolean {
        return this.isNotEqualTo(value, undefined, message);
    }

    isTrue(value: boolean, message?: string): boolean {
        return this.areExact(value, true, message);
    }

    isTruthy(value: any, message?: string): boolean {
        return this.isTrue(!!value, message);
    }

    isFalse(value: boolean, message?: string): boolean {
        return this.areExact(value, false, message);
    }

    isFalsey(value: any, message?: string): boolean {
        return this.isFalse(!!value, message);
    }

    isBoolean(value: unknown, message?: string): boolean {
        return this.areExact(value === true || value === false, true, message);
    }

    hasValue(value: any, message?: string): boolean {
        message = message || value;
        const goodMessage = `${message} is a value`,
            badMessage = `${message} is not a value`,
            isValue = value !== null
                && value !== undefined;
        return this.isTrue(isValue, isValue ? goodMessage : badMessage);
    }

    isEmpty(value: string, message?: string): boolean {
        message = message || `${value} is empty`;
        return this.areExact(value.length, 0, message);
    }

    isNotEqualTo<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, "!==", expected, (a, b) => a !== b, message);
    }

    areDifferent<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, "!=", expected, (a, b) => a != b, message);
    }

    isLessThan<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, "<", expected, (a, b) => a < b, message);
    }

    isLessThanEqual<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, "<=", expected, (a, b) => a <= b, message);
    }

    isGreaterThan<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, ">", expected, (a, b) => a > b, message);
    }

    isGreaterThanEqual<T>(actual: T, expected: T, message?: string): boolean {
        return this.twoValueTest(actual, ">=", expected, (a, b) => a >= b, message);
    }

    throws(func: Function, message?: string): Promise<boolean> {
        return this.throwTest(func, true, message);
    }

    doesNotThrow(func: Function, message?: string): Promise<boolean> {
        return this.throwTest(func, false, message);
    }

    async resolves(task: Promise<any>) {
        await this.doesNotThrow(() => task);
    }

    async rejects(task: Promise<any>) {
        await this.throws(() => task);
    }

    private twoValueTest<T>(actual: T, op: string, expected: T, testFunc: (a: T, b: T) => boolean, message?: string): boolean {
        if (testFunc(actual, expected)) {
            return this.success();
        }
        else {
            return this.fail(`${message || ""} [Actual: ${actual}] ${op} [Expected: ${expected}]`);
        }
    }

    private async throwTest(func: Function, op: boolean, message?: string): Promise<boolean> {
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
            return this.success();
        }
        else {
            return this.fail(message);
        }
    }
}

export interface TestCaseConstructor extends Constructor<TestCase, typeof TestCase> { }