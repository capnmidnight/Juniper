export declare class Exception extends Error {
    readonly innerError: any;
    constructor(message: string, innerError?: any);
}
export declare function assert(name: string, value: any, expected: any): void;
export declare class AssertFailedException extends Exception {
    #private;
    constructor(name: string, message: string);
    get name(): string;
}
export declare function withErrorAlert(action: () => (void | Promise<void>)): Promise<void>;
//# sourceMappingURL=Exception.d.ts.map