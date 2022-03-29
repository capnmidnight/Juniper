import { alwaysTrue } from "../identity";
import { isDefined, isNullOrUndefined } from "../typeChecks";

export class Task<T = void> implements Promise<T> {

    private readonly promise: Promise<T>;

    private _resolve: (value: T) => void = null;
    private _reject: (reason: any) => void = null;
    private _result: T = null;
    private _error: unknown = null;

    get result(): T {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): unknown {
        return this._error;
    }

    readonly resolve: (value: T) => void = null;
    readonly reject: (reason: any) => void = null;

    constructor(resolveTest?: (value: T) => boolean, rejectTest?: (reason: any) => boolean) {

        resolveTest = resolveTest || alwaysTrue;
        rejectTest = rejectTest || alwaysTrue;

        this.resolve = (value: T): void => {
            if (isDefined(this._resolve)) {
                this._resolve(value);
            }
        };

        this.reject = (reason: unknown): void => {
            if (isDefined(this._reject)) {
                this._reject(reason);
            }
        };

        this.promise = new Promise((resolve, reject) => {
            this._resolve = (value: T) => {
                if (resolveTest(value)) {
                    this._result = value;
                    resolve(value);
                }
            };

            this._reject = (reason: any) => {
                if (rejectTest(reason)) {
                    this._error = reason;
                    reject(reason);
                }
            };
        });
    }

    get [Symbol.toStringTag](): string {
        return this.promise.toString();
    }

    then<TResult1 = T, TResult2 = never>(onfulfilled?: (value: T) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<T | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<T> {
        return this.promise.finally(onfinally);
    }
}

export class Promisifier<T = void> implements Promise<T> {

    private readonly promise: Promise<T>;

    callback: (...args: any[]) => void = null;

    constructor(
        resolveRejectTest: (...args: any[]) => boolean,
        selectValue: (...args: any[]) => T,
        selectRejectionReason: (...args: any[]) => any) {
        this.promise = new Promise((resolve, reject) => {
            this.callback = (...args: any[]) => {
                if (resolveRejectTest(...args)) {
                    resolve(selectValue(...args));
                }
                else {
                    reject(selectRejectionReason(...args));
                }
            };
        });

    }

    get [Symbol.toStringTag](): string {
        return this.promise.toString();
    }

    then<TResult1 = T, TResult2 = never>(onfulfilled?: (value: T) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<T | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<T> {
        return this.promise.finally(onfinally);
    }
}