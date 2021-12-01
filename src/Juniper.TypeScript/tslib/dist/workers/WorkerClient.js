import { TypedEvent, TypedEventBase } from "../events/EventBase";
import { isProgressCallback } from "../progress/IProgress";
import { assertNever, isArray, isDefined } from "../typeChecks";
export class WorkerClient extends TypedEventBase {
    worker;
    static isSupported = "Worker" in globalThis;
    taskCounter = 0;
    invocations = new Map();
    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(worker) {
        super();
        this.worker = worker;
        if (!WorkerClient.isSupported) {
            console.warn("Workers are not supported on this system.");
        }
        this.worker.addEventListener("message", (evt) => {
            const data = evt.data;
            switch (data.type) {
                case "event":
                    this.propogateEvent(data);
                    break;
                case "progress":
                    this.progressReport(data);
                    break;
                case "return":
                    this.methodReturned(data);
                    break;
                case "error":
                    this.invocationError(data);
                    break;
                default:
                    assertNever(data);
            }
        });
    }
    postMessage(message, transferables) {
        if (message.type !== "methodCall") {
            assertNever(message.type);
        }
        if (transferables) {
            this.worker.postMessage(message, transferables);
        }
        else {
            this.worker.postMessage(message);
        }
    }
    dispose() {
        this.worker.terminate();
    }
    propogateEvent(data) {
        const evt = new TypedEvent(data.eventName);
        this.dispatchEvent(Object.assign(evt, data.data));
    }
    progressReport(data) {
        const invocation = this.invocations.get(data.taskID);
        const { onProgress } = invocation;
        if (onProgress) {
            onProgress.report(data.soFar, data.total, data.msg, data.est);
        }
    }
    methodReturned(data) {
        const messageHandler = this.removeInvocation(data.taskID);
        const { resolve } = messageHandler;
        resolve(data.returnValue);
    }
    invocationError(data) {
        const messageHandler = this.removeInvocation(data.taskID);
        const { reject, methodName } = messageHandler;
        reject(new Error(`${methodName} failed. Reason: ${data.errorMessage}`));
    }
    /**
     * When the invocation has errored, we want to stop listening to the worker
     * message channel so we don't eat up processing messages that have no chance
     * ever pertaining to the invocation.
     **/
    removeInvocation(taskID) {
        const invocation = this.invocations.get(taskID);
        this.invocations.delete(taskID);
        return invocation;
    }
    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param parameters - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param onProgress - a callback for receiving progress reports on long-running invocations.
     */
    callMethod(methodName, parameters, transferables, onProgress) {
        if (!WorkerClient.isSupported) {
            return Promise.reject(new Error("Workers are not supported on this system."));
        }
        // Normalize method parameters.
        let params = null;
        let tfers = null;
        if (isProgressCallback(parameters)) {
            onProgress = parameters;
            parameters = null;
            transferables = null;
        }
        if (isProgressCallback(transferables)
            && !onProgress) {
            onProgress = transferables;
            transferables = null;
        }
        if (isArray(parameters)) {
            params = parameters;
        }
        if (isArray(transferables)) {
            tfers = transferables;
        }
        // taskIDs help us keep track of return values.
        const taskID = this.taskCounter++;
        return new Promise((resolve, reject) => {
            const invocation = {
                onProgress,
                resolve,
                reject,
                methodName
            };
            this.invocations.set(taskID, invocation);
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
            this.postMessage(message, tfers);
        });
    }
}
