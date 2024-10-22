import { Task } from "./Task";
import { TypedEvent } from './TypedEventTarget';
/**
 * A Task represents a Promise that exposes its resolve/reject functions
 * as methods, rather than requiring a callback being passed to its constructor.
 * Tasks can be used to build manually-resolved Promises with less
 * boilerplate of nested function blocks.
 **/
export class TaskEvent extends TypedEvent {
    #task;
    /**
     * Create a new Task
     *
     * @param autoStart - set to false to require manually starting the Task. Useful
     * for reusable tasks that run on timers.
     */
    constructor(type, options) {
        super(type, options);
        this.#task = new Task(options?.autoStart);
        // It's very likely that we will want to use resolve/reject
        // as values to pass to another function/method, so we create
        // them not as methods, but as bound lambda expressions stored
        // in public fields.
        this.resolve = (value) => this.#task.resolve(value);
        this.reject = (reason) => this.#task.reject(reason);
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
    resolver(value) {
        return this.#task.resolver(value);
    }
    resolveOn(target, resolveEvt, value) {
        this.#task.resolveOn(target, resolveEvt, value);
    }
    /**
     * Get the last result that the task had resolved to, if any is available.
     *
     * If the Task had been rejected, attempting to get the result will rethrow
     * the error that had rejected the task.
     **/
    get result() { return this.#task.result; }
    /**
     * Get the last error that the task had been rejected by, if any.
     **/
    get error() { return this.#task.error; }
    /**
     * Get the current state of the task.
     **/
    get executionState() { return this.#task.executionState; }
    /**
     * Returns true when the Task is hasn't started yet.
     **/
    get waiting() { return this.#task.waiting; }
    /**
     * Returns true when the Task is waiting to be resolved or rejected.
     **/
    get started() { return this.#task.started; }
    /**
     * Returns true after the Task has started, but before it has finished.
     **/
    get running() { return this.#task.running; }
    /**
     * Returns true when the Task has been resolved or rejected.
     **/
    get finished() { return this.#task.finished; }
    get resultState() { return this.#task.resultState; }
    /**
     * Returns true if the Task had been resolved successfully.
     **/
    get resolved() { return this.#task.resolved; }
    /**
     * Returns true if the Task had been rejected, regardless of any
     * reason being given.
     **/
    get errored() { return this.#task.errored; }
    get [Symbol.toStringTag]() {
        return this.toString();
    }
    /**
     * Attach a handler to the task that fires when the task is resolved.
     *
     * @param onfulfilled
     * @param onrejected
     */
    then(onfulfilled, onrejected) {
        return this.#task.then(onfulfilled, onrejected);
    }
    /**
     * Attach a handler that fires when the Task is rejected.
     *
     * @param onrejected
     */
    catch(onrejected) {
        return this.#task.catch(onrejected);
    }
    /**
     * Attach a handler that fires regardless of whether the Task is resolved
     * or rejected.
     *
     * @param onfinally
     */
    finally(onfinally) {
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
//# sourceMappingURL=TaskEvent.js.map