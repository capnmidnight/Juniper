import { isBoolean, isDefined, isFunction } from "../";
import { arrayClear } from "../collections";
import { alwaysTrue } from "../identity";
import { Predicate } from "./Predicate";



export class Task<ResultsT = void> implements Promise<ResultsT> {
    private readonly onThens = new Array<(v: ResultsT) => any>();
    private readonly onCatches = new Array<(reason?: any) => void>();
    private readonly rejectTest: Predicate<any>;
    private readonly resolveTest: Predicate<ResultsT>;
    private readonly autoStart: boolean;

    private _result: ResultsT = null;
    private _error: any = null;
    private _started = false;
    private _errored = false;
    private _finished = false;

    get result(): ResultsT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    get error(): any {
        return this._error;
    }

    get started(): boolean {
        return this._started;
    }

    get finished(): boolean {
        return this._finished;
    }

    get errored(): boolean {
        return this._errored;
    }

    constructor(autoStart?: boolean);
    constructor(resolveTest: Predicate<ResultsT>, autoStart?: boolean)
    constructor(resolveTest: Predicate<ResultsT>, rejectTest: Predicate<any>, autoStart?: boolean);
    constructor(resolveTestOrAutoStart?: boolean | Predicate<ResultsT>, rejectTestOrAutoStart?: boolean | Predicate<any>, autoStart = true) {
        if (isFunction(resolveTestOrAutoStart)) {
            this.resolveTest = resolveTestOrAutoStart;
        }
        else {
            this.resolveTest = alwaysTrue;
        }

        if (isFunction(rejectTestOrAutoStart)) {
            this.rejectTest = rejectTestOrAutoStart;
        }
        else {
            this.rejectTest = alwaysTrue;
        }

        if (isBoolean(resolveTestOrAutoStart)) {
            this.autoStart = resolveTestOrAutoStart;
        }
        else if (isBoolean(rejectTestOrAutoStart)) {
            this.autoStart = rejectTestOrAutoStart;
        }
        else if (isDefined(autoStart)) {
            this.autoStart = autoStart;
        }
        else {
            this.autoStart = false;
        }

        if (this.autoStart) {
            this.start();
        }
    }

    start() {
        this._started = true;
    }

    resolve(value: ResultsT): void {
        if (this.started
            && !this.finished
            && this.resolveTest(value)) {
            this._result = value;
            for (const thenner of this.onThens) {
                thenner(value);
            }
            this._finished = true;
        }
    }

    reject(reason: any): void {
        if (this.started
            && !this.finished
            && this.rejectTest(reason)) {
            this._error = reason;
            this._errored = true;
            for (const catcher of this.onCatches) {
                catcher(reason);
            }
            this._finished = true;
        }
    }

    get [Symbol.toStringTag](): string {
        return this.toString();
    }

    private project(): Promise<ResultsT> {
        return new Promise<ResultsT>((resolve, reject) => {
            if (!this.finished) {
                this.onThens.push(resolve);
                this.onCatches.push(reject);
            }
            else if (this.errored) {
                reject(this.error);
            }
            else {
                resolve(this.result);
            }
        });
    }

    then<TResult1 = ResultsT, TResult2 = never>(onfulfilled?: (value: ResultsT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.project().then(onfulfilled, onrejected);
    }

    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultsT | TResult> {
        return this.project().catch(onrejected);
    }

    finally(onfinally?: () => void): Promise<ResultsT> {
        return this.project().finally(onfinally);
    }

    reset() {
        if (this.started && !this.finished) {
            this.reject("Resetting previous invocation");
        }

        arrayClear(this.onThens);
        arrayClear(this.onCatches);
        this._started = this.autoStart;
        this._errored = false;
        this._finished = false;
    }
}