import { alwaysTrue } from "../identity";
import { isBoolean, isDefined, isFunction } from "../typeChecks";

type Predicate<T> = (value: T) => boolean;

export class Task<T = void> implements Promise<T> {

    private readonly promise: Promise<T>;

    private _resolve: (value: T) => void = null;
    private _reject: (reason: any) => void = null;
    private _result: T = null;
    private _error: unknown = null;
    private _started = false;
    private _finished = false;

    get result(): T {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): unknown {
        return this._error;
    }

    get started(): boolean {
        return this._started;
    }

    get finished(): boolean {
        return this._finished;
    }

    readonly resolve: (value: T) => void = null;
    readonly reject: (reason: any) => void = null;

    constructor(autoStart?: boolean);
    constructor(resolveTest: Predicate<T>, autoStart?: boolean)
    constructor(resolveTest: Predicate<T>, rejectTest: Predicate<any>, autoStart?: boolean);
    constructor(resolveTestOrAutoStart?: boolean | Predicate<T>, rejectTestOrAutoStart?: boolean | Predicate<any>, autoStart = true) {
        let resolveTest: Predicate<T> = alwaysTrue;
        let rejectTest: Predicate<any> = alwaysTrue;

        if (isFunction(resolveTestOrAutoStart)) {
            resolveTest = resolveTestOrAutoStart;
        }

        if (isFunction(rejectTestOrAutoStart)) {
            rejectTest = rejectTestOrAutoStart;
        }

        if (isBoolean(resolveTestOrAutoStart)) {
            autoStart = resolveTestOrAutoStart;
        }
        else if (isBoolean(rejectTestOrAutoStart)) {
            autoStart = rejectTestOrAutoStart;
        }

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
                    this._finished = true;
                    resolve(value);
                }
            };

            this._reject = (reason: any) => {
                if (rejectTest(reason)) {
                    this._error = reason;
                    this._finished = true;
                    reject(reason);
                }
            };
        });

        if (autoStart) {
            this.start();
        }
    }

    start() {
        this._started = true;
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