export declare class Promisifier<T = void> implements Promise<T> {
    #private;
    callback: (...args: any[]) => void;
    constructor(resolveRejectTest: (...args: any[]) => boolean, selectValue: (...args: any[]) => T, selectRejectionReason: (...args: any[]) => any);
    get [Symbol.toStringTag](): string;
    then<TResult1 = T, TResult2 = never>(onfulfilled?: (value: T) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2>;
    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<T | TResult>;
    finally(onfinally?: () => void): Promise<T>;
}
//# sourceMappingURL=Promisifier.d.ts.map