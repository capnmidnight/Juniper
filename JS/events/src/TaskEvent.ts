import { Task } from "./Task";
import { TypedEvent, TypedEventMap, TypedEventTarget } from './TypedEventTarget';

interface TaskEventInit extends EventInit {
    autoStart: boolean;
}

/**
 * A Task represents a Promise that exposes its resolve/reject functions
 * as methods, rather than requiring a callback being passed to its constructor.
 * Tasks can be used to build manually-resolved Promises with less
 * boilerplate of nested function blocks.
 **/
export class TaskEvent<EventT extends string, ResultsT = void>
    extends TypedEvent<EventT>
    implements Promise<ResultsT> {

    readonly #task: Task<ResultsT>;

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
    constructor(type: EventT, options?: Partial<TaskEventInit>) {
        super(type, options);

        this.#task = new Task(options?.autoStart);

        // It's very likely that we will want to use resolve/reject
        // as values to pass to another function/method, so we create
        // them not as methods, but as bound lambda expressions stored
        // in public fields.
        this.resolve = (value) =>
            this.#task.resolve(value);

        this.reject = (reason) =>
            this.#task.reject(reason);
    }

    /**
     * If the task was not auto-started, signal that the task is now ready to recieve
     * resolutions or rejections.
     **/
    start() {
        this.#task.start();
    }

    /**
     * Creates a resolving callback for a static value.
     * @param value
     */
    resolver(value: ResultsT) {
        return this.#task.resolver(value);
    }

    resolveOn<EventMapT extends TypedEventMap<string>, EventT extends keyof EventMapT = keyof EventMapT>(
        target: TypedEventTarget<EventMapT> | EventTarget,
        resolveEvt: EventT,
        value: ResultsT) {
        this.#task.resolveOn(target, resolveEvt, value);
    }

    /**
     * Get the last result that the task had resolved to, if any is available.
     *
     * If the Task had been rejected, attempting to get the result will rethrow
     * the error that had rejected the task.
     **/
    get result(): ResultsT { return this.#task.result }

    /**
     * Get the last error that the task had been rejected by, if any.
     **/
    get error(): any { return this.#task.error; }

    /**
     * Get the current state of the task.
     **/
    get executionState() { return this.#task.executionState; }

    /**
     * Returns true when the Task is hasn't started yet.
     **/
    get waiting(): boolean { return this.#task.waiting; }

    /**
     * Returns true when the Task is waiting to be resolved or rejected.
     **/
    get started(): boolean { return this.#task.started; }

    /**
     * Returns true after the Task has started, but before it has finished.
     **/
    get running(): boolean { return this.#task.running; }

    /**
     * Returns true when the Task has been resolved or rejected.
     **/
    get finished(): boolean { return this.#task.finished; }

    get resultState() { return this.#task.resultState; }

    /**
     * Returns true if the Task had been resolved successfully.
     **/
    get resolved(): boolean { return this.#task.resolved; }

    /**
     * Returns true if the Task had been rejected, regardless of any
     * reason being given.
     **/
    get errored(): boolean { return this.#task.errored; }

    get [Symbol.toStringTag](): string {
        return this.toString();
    }

    /**
     * Attach a handler to the task that fires when the task is resolved.
     * 
     * @param onfulfilled
     * @param onrejected
     */
    then<TResult1 = ResultsT, TResult2 = never>(onfulfilled?: (value: ResultsT) => TResult1 | PromiseLike<TResult1>, onrejected?: (reason: any) => TResult2 | PromiseLike<TResult2>): Promise<TResult1 | TResult2> {
        return this.#task.then(onfulfilled, onrejected);
    }

    /**
     * Attach a handler that fires when the Task is rejected.
     * 
     * @param onrejected
     */
    catch<TResult = never>(onrejected?: (reason: any) => TResult | PromiseLike<TResult>): Promise<ResultsT | TResult> {
        return this.#task.catch(onrejected);
    }

    /**
     * Attach a handler that fires regardless of whether the Task is resolved
     * or rejected.
     * 
     * @param onfinally
     */
    finally(onfinally?: () => void): Promise<ResultsT> {
        return this.#task.finally(onfinally);
    }

    /**
     * Resets the Task to an unsignalled state, which is useful for
     * reducing GC pressure when working with lots of tasks.
     **/
    reset() {
        this.#task.reset();
    }

    restart() {
        this.#task.restart();
    }
}