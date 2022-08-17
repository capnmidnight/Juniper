import { arrayClear } from "../collections/arrayClear";
import { alwaysTrue } from "../identity";
import { isBoolean, isDefined, isFunction } from "../typeChecks";
import { Predicate } from "./Predicate";

/**
 * A Task represents a Promise that exposes its resolve/reject functions
 * as methods, rather than requiring a callback being passed to its constructor.
 * Tasks can be used to build manually-resolved Promises with less
 * boilerplate of nested function blocks.
 **/
export class Task<ResultsT = void> implements Promise<ResultsT> {
    private readonly onThens = new Array<(v: ResultsT) => any>();
    private readonly onCatches = new Array<(reason?: any) => void>();

    private readonly rejectTest: Predicate<any>;
    private readonly resolveTest: Predicate<ResultsT>;
    private readonly autoStart: boolean;

    private _result: ResultsT = undefined;
    private _error: any = undefined;
    private _started = false;
    private _errored = false;
    private _finished = false;

    /**
     * Signal success for the Task
     *
     * @param value - the value to store with the resolved Task.
     **/
    public readonly resolve: (value: ResultsT) => void;

    /**
     * Signal failrue for the Task
     *
     * @param value - the error to store with the rejected Task.
     **/
    public readonly reject: (reason: any) => void;


    /**
     * Create a new Task
     *
     * @param autoStart - set to false to require manually starting the Task. Useful
     * for reusable tasks that run on timers.
     */
    constructor(autoStart?: boolean);

    /**
     * Create a new Task
     * 
     * @param resolveTest - a filtering function for values passed to Task.resolve()
     * to only resolve the Task for values that pass the filter. This is useful when
     * connecting the task to an event handler that may fire multiple events that
     * aren't of interest, such as Tasks that listen for a specific keyboard key
     * to be pressed.
     *
     * @param autoStart - set to false to require manually starting the Task. Useful
     * for reusable tasks that run on timers.
     */
    constructor(resolveTest: Predicate<ResultsT>, autoStart?: boolean)

    /**
     * Create a new Task
     * 
     * @param resolveTest - a filtering function for values passed to Task.resolve()
     * to only resolve the Task for values that pass the filter. This is useful when
     * connecting the task to an event handler that may fire multiple events that
     * aren't of interest, such as Tasks that listen for a specific keyboard key
     * to be pressed.
     * 
     * @param rejectTest - a filtering function for error reasons passed to Task.reject()
     * to only reject the Task when errors pass the filter.
     *
     * @param autoStart - set to false to require manually starting the Task. Useful
     * for reusable tasks that run on timers.
     */
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

        // It's very likely that we will want to use resolve/reject
        // as values to pass to another function/method, so we create
        // them not as methods, but as bound lambda expressions stored
        // in public fields.
        this.resolve = (value) => {
            if (this.running && this.resolveTest(value)) {
                this._result = value;
                for (const thenner of this.onThens) {
                    thenner(value);
                }
                this.clear();
                this._finished = true;
            }
        };

        this.reject = (reason) => {
            if (this.running && this.rejectTest(reason)) {
                this._error = reason;
                this._errored = true;
                for (const catcher of this.onCatches) {
                    catcher(reason);
                }
                this.clear();
                this._finished = true;
            }
        };

        if (this.autoStart) {
            this.start();
        }
    }

    private clear() {
        arrayClear(this.onThens);
        arrayClear(this.onCatches);
    }

    /**
     * If the task was not auto-started, signal that the task is now ready to recieve
     * resolutions or rejections.
     **/
    start() {
        this._started = true;
    }

    /**
     * Get the last result that the task had resolved to, if any is available.
     *
     * If the Task had been rejected, attempting to get the result will rethrow
     * the error that had rejected the task.
     **/
    get result(): ResultsT {
        if (isDefined(this.error)) {
            throw this.error;
        }

        return this._result;
    }

    /**
     * Get the last error that the task had been rejected by, if any.
     **/
    get error(): any {
        return this._error;
    }

    /**
     * Returns true when the Task is waiting to be resolved or rejected.
     **/
    get started(): boolean {
        return this._started;
    }

    /**
     * Returns true when the Task has been resolved or rejected.
     **/
    get finished(): boolean {
        return this._finished;
    }

    /**
     * Returns true after the Task has started, but before it has finished.
     **/
    get running(): boolean {
        return this.started && !this.finished;
    }

    /**
     * Returns true if the Task had been rejected, regardless of any
     * reason being given.
     **/
    get errored(): boolean {
        return this._errored;
    }

    get [Symbol.toStringTag](): string {
        return this.toString();
    }

    /**
     * Calling Task.then(), Task.catch(), or Task.finally() creates a new Promise.
     * This method creates that promise and links it with the task.
     **/
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

    /**
     * Attach a handler to the task that fires when the task is resolved.
     * 
     * @param onfulfilled
     * @param onrejected
     */
    then<TResult1 = ResultsT, TResult2 = never>(onfulfilled?: (value: ResultsT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.project().then(onfulfilled, onrejected);
    }

    /**
     * Attach a handler that fires when the Task is rejected.
     * 
     * @param onrejected
     */
    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultsT | TResult> {
        return this.project().catch(onrejected);
    }

    /**
     * Attach a handler that fires regardless of whether the Task is resolved
     * or rejected.
     * 
     * @param onfinally
     */
    finally(onfinally?: () => void): Promise<ResultsT> {
        return this.project().finally(onfinally);
    }

    /**
     * Resets the Task to an unsignalled state, which is useful for
     * reducing GC pressure when working with lots of tasks.
     **/
    reset() {
        if (this.running) {
            this.reject("Resetting previous invocation");
        }

        this.clear();
        this._result = undefined;
        this._error = undefined;
        this._errored = false;
        this._finished = false;
        this._started = false;

        if (this.autoStart) {
            this.start();
        }
    }
}