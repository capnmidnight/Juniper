import { TypedEventBase } from "juniper-tslib";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestCaseSuccessEvent } from "./TestCaseSuccessEvent";
interface TestCaseEvents {
    testcasemessage: TestCaseMessageEvent;
    testcasesuccess: TestCaseSuccessEvent;
    testcasefail: TestCaseFailEvent;
}
export declare class TestCase extends TypedEventBase<TestCaseEvents> {
    setup(): void;
    teardown(): void;
    message(msg: string): void;
    success(): void;
    fail(msg: string): void;
    isEqualTo<T>(actual: T, expected: T, message?: string): void;
    isNull(value: any, message?: string): void;
    isNotNull(value: any, message?: string): void;
    isUndefined(value: any, message?: string): void;
    isNotUndefined(value: any, message?: string): void;
    isTrue(value: boolean, message?: string): void;
    isFalse(value: boolean, message?: string): void;
    isBoolean(value: unknown, message?: string): void;
    hasValue(value: any, message?: string): void;
    isEmpty(value: string, message?: string): void;
    isNotEqualTo<T>(actual: T, expected: T, message?: string): void;
    isLessThan<T>(actual: T, expected: T, message?: string): void;
    isLessThanEqual<T>(actual: T, expected: T, message?: string): void;
    isGreaterThan<T>(actual: T, expected: T, message?: string): void;
    isGreaterThanEqual<T>(actual: T, expected: T, message?: string): void;
    throws(func: Function, message?: string): void;
    doesNotThrow(func: Function, message?: string): void;
    private twoValueTest;
    private throwTest;
}
export {};
