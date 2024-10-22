import { TypedEventTarget, TypedEventMap } from "./TypedEventTarget";
export type TaskExecutionState = "waiting" | "running" | "finished";
export type TaskResultState = "none" | "resolved" | "errored";
/**
 * A Task represents a Promise that exposes its resolve/reject functions
 * as methods, rather than requiring a callback being passed to its constructor.
 * Tasks can be used to build manually-resolved Promises with less
 * boilerplate of nested function blocks.
 **/
export declare class Task<ResultsT = void> implements Promise<ResultsT> {
    #private;
    /**
     * Signal success for the Task
     *
     * @param value - the value to store with the resolved Task.
     **/
    readonly resolve: (value: ResultsT) => void;
    /**
     * Signal failrue for the Task
     *
     * @param value - the error to store with the rejected Task.
     **/
    readonly reject: (reason: any) => void;
    /**
     * Create a new Task
     *
     * @param autoStart - set to false to require manually starting the Task. Useful
     * for reusable tasks that run on timers.
     */
    constructor(autoStart?: boolean);
    /**
     * If the task was not auto-started, signal that the task is now ready to recieve
     * resolutions or rejections.
     **/
    start(): void;
    /**
     * Creates a resolving callback for a static value.
     * @param value
     */
    resolver(value: ResultsT): () => void;
    resolveOn<EventMapT extends TypedEventMap<string>, EventT extends keyof EventMapT = keyof EventMapT>(target: TypedEventTarget<EventMapT> | EventTarget, resolveEvt: EventT, value: ResultsT): void;
    /**
     * Get the last result that the task had resolved to, if any is available.
     *
     * If the Task had been rejected, attempting to get the result will rethrow
     * the error that had rejected the task.
     **/
    get result(): ResultsT;
    /**
     * Get the last error that the task had been rejected by, if any.
     **/
    get error(): any;
    /**
     * Get the current state of the task.
     **/
    get executionState(): TaskExecutionState;
    /**
     * Returns true when the Task is hasn't started yet.
     **/
    get waiting(): boolean;
    /**
     * Returns true when the Task is waiting to be resolved or rejected.
     **/
    get started(): boolean;
    /**
     * Returns true after the Task has started, but before it has finished.
     **/
    get running(): boolean;
    /**
     * Returns true when the Task has been resolved or rejected.
     **/
    get finished(): boolean;
    get resultState(): TaskResultState;
    /**
     * Returns true if the Task had been resolved successfully.
     **/
    get resolved(): boolean;
    /**
     * Returns true if the Task had been rejected, regardless of any
     * reason being given.
     **/
    get errored(): boolean;
    get [Symbol.toStringTag](): string;
    /**
     * Attach a handler to the task that fires when the task is resolved.
     *
     * @param onfulfilled
     * @param onrejected
     */
    then<TResult1 = ResultsT, TResult2 = never>(onfulfilled?: (value: ResultsT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2>;
    /**
     * Attach a handler that fires when the Task is rejected.
     *
     * @param onrejected
     */
    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultsT | TResult>;
    /**
     * Attach a handler that fires regardless of whether the Task is resolved
     * or rejected.
     *
     * @param onfinally
     */
    finally(onfinally?: () => void): Promise<ResultsT>;
    /**
     * Resets the Task to an unsignalled state, which is useful for
     * reducing GC pressure when working with lots of tasks.
     **/
    reset(): void;
    restart(): void;
}
//# sourceMappingURL=Task.d.ts.map