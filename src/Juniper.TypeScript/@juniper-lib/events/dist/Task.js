import { arrayClear } from "@juniper-lib/collections/dist/arrays";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
/**
 * A Task represents a Promise that exposes its resolve/reject functions
 * as methods, rather than requiring a callback being passed to its constructor.
 * Tasks can be used to build manually-resolved Promises with less
 * boilerplate of nested function blocks.
 **/
export class Task {
    /**
     * Create a new Task
     *
     * @param autoStart - set to false to require manually starting the Task. Useful
     * for reusable tasks that run on timers.
     */
    constructor(autoStart = true) {
        this.autoStart = autoStart;
        this.onThens = new Array();
        this.onCatches = new Array();
        this._result = undefined;
        this._error = undefined;
        this._executionState = "waiting";
        this._resultState = "none";
        // It's very likely that we will want to use resolve/reject
        // as values to pass to another function/method, so we create
        // them not as methods, but as bound lambda expressions stored
        // in public fields.
        this.resolve = (value) => {
            if (this.running) {
                this._result = value;
                this._resultState = "resolved";
                for (const thenner of this.onThens) {
                    thenner(value);
                }
                this.clear();
                this._executionState = "finished";
            }
        };
        this.reject = (reason) => {
            if (this.running) {
                this._error = reason;
                this._resultState = "errored";
                for (const catcher of this.onCatches) {
                    catcher(reason);
                }
                this.clear();
                this._executionState = "finished";
            }
        };
        if (this.autoStart) {
            this.start();
        }
    }
    clear() {
        arrayClear(this.onThens);
        arrayClear(this.onCatches);
    }
    /**
     * If the task was not auto-started, signal that the task is now ready to recieve
     * resolutions or rejections.
     **/
    start() {
        this._executionState = "running";
    }
    /**
     * Creates a resolving callback for a static value.
     * @param value
     */
    resolver(value) {
        return () => this.resolve(value);
    }
    resolveOn(target, resolveEvt, value) {
        const resolver = this.resolver(value);
        target.addEventListener(resolveEvt, resolver);
        this.finally(() => target.removeEventListener(resolveEvt, resolver));
    }
    /**
     * Get the last result that the task had resolved to, if any is available.
     *
     * If the Task had been rejected, attempting to get the result will rethrow
     * the error that had rejected the task.
     **/
    get result() {
        if (isDefined(this.error)) {
            throw this.error;
        }
        return this._result;
    }
    /**
     * Get the last error that the task had been rejected by, if any.
     **/
    get error() {
        return this._error;
    }
    /**
     * Get the current state of the task.
     **/
    get executionState() {
        return this._executionState;
    }
    /**
     * Returns true when the Task is hasn't started yet.
     **/
    get waiting() {
        return this.executionState === "waiting";
    }
    /**
     * Returns true when the Task is waiting to be resolved or rejected.
     **/
    get started() {
        return this.executionState !== "waiting";
    }
    /**
     * Returns true after the Task has started, but before it has finished.
     **/
    get running() {
        return this.executionState === "running";
    }
    /**
     * Returns true when the Task has been resolved or rejected.
     **/
    get finished() {
        return this.executionState === "finished";
    }
    get resultState() {
        return this._resultState;
    }
    /**
     * Returns true if the Task had been resolved successfully.
     **/
    get resolved() {
        return this.resultState === "resolved";
    }
    /**
     * Returns true if the Task had been rejected, regardless of any
     * reason being given.
     **/
    get errored() {
        return this.resultState === "errored";
    }
    get [Symbol.toStringTag]() {
        return this.toString();
    }
    /**
     * Calling Task.then(), Task.catch(), or Task.finally() creates a new Promise.
     * This method creates that promise and links it with the task.
     **/
    project() {
        return new Promise((resolve, reject) => {
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
    then(onfulfilled, onrejected) {
        return this.project().then(onfulfilled, onrejected);
    }
    /**
     * Attach a handler that fires when the Task is rejected.
     *
     * @param onrejected
     */
    catch(onrejected) {
        return this.project().catch(onrejected);
    }
    /**
     * Attach a handler that fires regardless of whether the Task is resolved
     * or rejected.
     *
     * @param onfinally
     */
    finally(onfinally) {
        return this.project().finally(onfinally);
    }
    /**
     * Resets the Task to an unsignalled state, which is useful for
     * reducing GC pressure when working with lots of tasks.
     **/
    reset() {
        this._reset(this.autoStart);
    }
    restart() {
        this._reset(true);
    }
    _reset(start) {
        if (this.running) {
            this.reject("Resetting previous invocation");
        }
        this.clear();
        this._result = undefined;
        this._error = undefined;
        this._executionState = "waiting";
        this._resultState = "none";
        if (start) {
            this.start();
        }
    }
}
//# sourceMappingURL=Task.js.map