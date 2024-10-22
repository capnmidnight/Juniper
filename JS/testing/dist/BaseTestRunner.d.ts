export declare class BaseTestRunner {
    test(name: string, args: string, ...callbacks: VoidFunction[]): void;
    log(..._msg: any[]): void;
    success(..._msg: any[]): void;
    fail(..._msg: any[]): void;
    error(_exp: Error, ..._msg: any[]): void;
}
//# sourceMappingURL=BaseTestRunner.d.ts.map