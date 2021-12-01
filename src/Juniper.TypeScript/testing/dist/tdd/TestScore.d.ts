import { TestStates } from "./TestStates";
export declare class TestScore {
    readonly name: string;
    state: TestStates;
    messages: string[];
    constructor(name: string);
    start(): void;
    success(): void;
    fail(message: string): void;
    finish(value: string): void;
}
