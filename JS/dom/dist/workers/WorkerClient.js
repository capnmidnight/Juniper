import { arrayScan, assertNever, isArray, isDefined } from "@juniper-lib/util";
import { Task, TypedEventTarget } from "@juniper-lib/events";
import { isProgressCallback } from "@juniper-lib/progress";
export class WorkerClient extends TypedEventTarget {
    #invocations = new Map();
    #tasks = new Array();
    #worker;
    #taskCounter = 0;
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(worker) {
        super();
        this.#worker = worker;
        this.#worker.addEventListener("message", (evt) => {
            const data = evt.data;
            switch (data.type) {
                case "event":
                    this.propogateEvent(data);
                    break;
                case "progress":
                    this.#progressReport(data);
                    break;
                case "return":
                    this.#methodReturned(data);
                    break;
                case "error":
                    this.#invocationError(data);
                    break;
                default:
                    assertNever(data);
            }
        });
    }
    #postMessage(message, transferables) {
        if (message.type !== "methodCall") {
            assertNever(message.type);
        }
        if (transferables) {
            this.#worker.postMessage(message, transferables);
        }
        else {
            this.#worker.postMessage(message);
        }
    }
    dispose() {
        this.#worker.terminate();
    }
    #progressReport(data) {
        const invocation = this.#invocations.get(data.taskID);
        if (invocation) {
            const { prog } = invocation;
            if (prog) {
                prog.report(data.soFar, data.total, data.msg, data.est);
            }
        }
    }
    #methodReturned(data) {
        const messageHandler = this.#removeInvocation(data.taskID);
        const { task } = messageHandler;
        task.resolve(data.returnValue);
    }
    #invocationError(data) {
        const messageHandler = this.#removeInvocation(data.taskID);
        const { task, methodName } = messageHandler;
        task.reject(new Error(`${methodName} failed. Reason: ${data.errorMessage}`));
    }
    /**
     * When the invocation has errored, we want to stop listening to the worker
     * message channel so we don't eat up processing messages that have no chance
     * ever pertaining to the invocation.
     **/
    #removeInvocation(taskID) {
        const invocation = this.#invocations.get(taskID);
        this.#invocations.delete(taskID);
        return invocation;
    }
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param parameters - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod(methodName, parameters, transferables, prog) {
        // Normalize method parameters.
        let params = null;
        let tfers = null;
        if (isProgressCallback(parameters)) {
            prog = parameters;
            parameters = null;
            transferables = null;
        }
        if (isProgressCallback(transferables)
            && !prog) {
            prog = transferables;
            transferables = null;
        }
        if (isArray(parameters)) {
            params = parameters;
        }
        if (isArray(transferables)) {
            tfers = transferables;
        }
        // taskIDs help us keep track of return values.
        const taskID = this.#taskCounter++;
        let task = arrayScan(this.#tasks, t => t.finished);
        if (task) {
            task.reset();
        }
        else {
            task = new Task();
            this.#tasks.push(task);
        }
        const invocation = {
            methodName,
            task,
            prog
        };
        this.#invocations.set(taskID, invocation);
        let message = null;
        if (isDefined(parameters)) {
            message = {
                type: "methodCall",
                taskID,
                methodName,
                params
            };
        }
        else {
            message = {
                type: "methodCall",
                taskID,
                methodName
            };
        }
        this.#postMessage(message, tfers);
        return task;
    }
}
//# sourceMappingURL=WorkerClient.js.map