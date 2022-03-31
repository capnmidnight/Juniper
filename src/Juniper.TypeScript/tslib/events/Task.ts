import { alwaysTrue } from "../identity";
import { isBoolean, isDefined, isFunction } from "../typeChecks";
import { Predicate } from "./Predicate";

export class Task<ResultsT = void, ErrorT = unknown> implements Promise<ResultsT> {

    private readonly promise: Promise<ResultsT>;

    private _resolve: (value: ResultsT) => void = null;
    private _reject: (reason: ErrorT) => void = null;
    private _result: ResultsT = null;
    private _error: ErrorT = null;
    private _started = false;
    private _finished = false;

    get result(): ResultsT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): ErrorT {
        return this._error;
    }

    get started(): boolean {
        return this._started;
    }

    get finished(): boolean {
        return this._finished;
    }

    readonly resolve: (value: ResultsT) => void = null;
    readonly reject: (reason: ErrorT) => void = null;

    constructor(autoStart?: boolean);
    constructor(resolveTest: Predicate<ResultsT>, autoStart?: boolean)
    constructor(resolveTest: Predicate<ResultsT>, rejectTest: Predicate<ErrorT>, autoStart?: boolean);
    constructor(resolveTestOrAutoStart?: boolean | Predicate<ResultsT>, rejectTestOrAutoStart?: boolean | Predicate<ErrorT>, autoStart = true) {
        let resolveTest: Predicate<ResultsT> = alwaysTrue;
        let rejectTest: Predicate<ErrorT> = alwaysTrue;

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

        this.resolve = (value: ResultsT): void => {
            if (isDefined(this._resolve)) {
                this._resolve(value);
            }
        };

        this.reject = (reason: ErrorT): void => {
            if (isDefined(this._reject)) {
                this._reject(reason);
            }
        };

        this.promise = new Promise((resolve, reject) => {
            this._resolve = (value: ResultsT) => {
                if (resolveTest(value)) {
                    this._result = value;
                    this._finished = true;
                    resolve(value);
                }
            };

            this._reject = (reason: ErrorT) => {
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

    then<TResult1 = ResultsT, TResult2 = never>(onfulfilled?: (value: ResultsT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.promise.then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultsT | TResult> {
        return this.promise.catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<ResultsT> {
        return this.promise.finally(onfinally);
    }
}