import { TypedEventTarget } from "@juniper-lib/events";
import { TestCaseFailEvent } from "./TestCaseFailEvent";
import { TestCaseMessageEvent } from "./TestCaseMessageEvent";
import { TestCaseSuccessEvent } from "./TestCaseSuccessEvent";
type TestCaseEvents = {
    testcasemessage: TestCaseMessageEvent;
    testcasesuccess: TestCaseSuccessEvent;
    testcasefail: TestCaseFailEvent;
};
export declare class TestCase extends TypedEventTarget<TestCaseEvents> {
    protected defaultError: number;
    setup(): void;
    teardown(): void;
    message(msg: string): void;
    success(): true;
    fail(msg: string): false;
    areSame(actual: any, expected: any, message?: string): boolean;
    areExact<T>(actual: T, expected: T, message?: string): boolean;
    arraysMatch<T>(actual: T[], expected: T[], message?: string): boolean;
    areApprox(actual: number, expected: number, expectedError?: number): boolean;
    areApprox(actual: number, expected: number, message: string, expectedError?: number): boolean;
    isNull(value: any, message?: string): boolean;
    isNotNull(value: any, message?: string): boolean;
    isUndefined(value: any, message?: string): boolean;
    isNotUndefined(value: any, message?: string): boolean;
    isTrue(value: boolean, message?: string): boolean;
    isTruthy(value: any, message?: string): boolean;
    isFalse(value: boolean, message?: string): boolean;
    isFalsey(value: any, message?: string): boolean;
    isBoolean(value: unknown, message?: string): boolean;
    hasValue(value: any, message?: string): boolean;
    isEmpty(value: string, message?: string): boolean;
    isNotEqualTo<T>(actual: T, expected: T, message?: string): boolean;
    areDifferent<T>(actual: T, expected: T, message?: string): boolean;
    isLessThan<T>(actual: T, expected: T, message?: string): boolean;
    isLessThanEqual<T>(actual: T, expected: T, message?: string): boolean;
    isGreaterThan<T>(actual: T, expected: T, message?: string): boolean;
    isGreaterThanEqual<T>(actual: T, expected: T, message?: string): boolean;
    throws(func: Function, message?: string): Promise<boolean>;
    doesNotThrow(func: Function, message?: string): Promise<boolean>;
    resolves(task: Promise<any>): Promise<void>;
    rejects(task: Promise<any>): Promise<void>;
    private twoValueTest;
    private throwTest;
}
export type TestCaseConstructor = Constructor<TestCase, typeof TestCase>;
export {};
//# sourceMappingURL=TestCase.d.ts.map