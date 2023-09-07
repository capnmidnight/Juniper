import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { isArray, isDefined } from "@juniper-lib/tslib/typeChecks";
class WorkerServerProgress extends BaseProgress {
    constructor(server, taskID) {
        super();
        this.server = server;
        this.taskID = taskID;
    }
    /**
     * Report progress through long-running invocations. If your invocable
     * functions don't report progress, this can be safely ignored.
     * @param soFar - how much of the process we've gone through.
     * @param total - the total amount we need to go through.
     * @param msg - an optional message to include as part of the progress update.
     * @param est - an optional estimate of how many milliseconds are left in the progress.
     */
    report(soFar, total, msg, est) {
        const message = {
            type: "progress",
            taskID: this.taskID,
            soFar,
            total,
            msg,
            est
        };
        this.server.postMessage(message);
    }
}
export class WorkerServer {
    /**
     * Creates a new worker thread method call listener.
     * @param self - the worker scope in which to listen.
     */
    constructor(self) {
        this.self = self;
        this.methods = new Map();
        this.self.addEventListener("message", (evt) => {
            const data = evt.data;
            this.callMethod(data);
        });
    }
    postMessage(message, transferables) {
        if (isDefined(transferables)) {
            this.self.postMessage(message, transferables);
        }
        else {
            this.self.postMessage(message);
        }
    }
    callMethod(data) {
        const method = this.methods.get(data.methodName);
        if (method) {
            try {
                if (isArray(data.params)) {
                    method(data.taskID, ...data.params);
                }
                else if (isDefined(data.params)) {
                    method(data.taskID, data.params);
                }
                else {
                    method(data.taskID);
                }
            }
            catch (exp) {
                this.onError(data.taskID, `method invocation error: ${data.methodName}(${exp.message || exp})`);
            }
        }
        else {
            this.onError(data.taskID, `method not found: ${data.methodName}`);
        }
    }
    /**
     * Report an error back to the calling thread.
     * @param taskID - the invocation ID of the method that errored.
     * @param errorMessage - what happened?
     */
    onError(taskID, errorMessage) {
        const message = {
            type: "error",
            taskID,
            errorMessage
        };
        this.postMessage(message);
    }
    /**
     * Return back to the client.
     * @param taskID - the invocation ID of the method that is returning.
     * @param returnValue - the (optional) value to return.
     * @param transferReturnValue - a mapping function to extract any Transferable objects from the return value.
     */
    onReturn(taskID, returnValue, transferReturnValue) {
        let message = null;
        if (returnValue === undefined) {
            message = {
                type: "return",
                taskID
            };
        }
        else {
            message = {
                type: "return",
                taskID,
                returnValue
            };
        }
        if (isDefined(transferReturnValue)) {
            const transferables = transferReturnValue(returnValue);
            this.postMessage(message, transferables);
        }
        else {
            this.postMessage(message);
        }
    }
    addMethodInternal(methodName, asyncFunc, transferReturnValue) {
        if (this.methods.has(methodName)) {
            throw new Error(`${methodName} method has already been mapped.`);
        }
        this.methods.set(methodName, async (taskID, ...params) => {
            const prog = new WorkerServerProgress(this, taskID);
            try {
                // Even functions returning void and functions returning bare, unPromised values, can be awaited.
                // This creates a convenient fallback where we don't have to consider the exact return type of the function.
                const returnValue = await asyncFunc(...params, prog);
                this.onReturn(taskID, returnValue, transferReturnValue);
            }
            catch (exp) {
                console.error(exp);
                this.onError(taskID, exp.message || exp);
            }
        });
    }
    /**
     * Registers a function call for cross-thread invocation.
     * @param methodName - the name of the function to use during invocations.
     * @param asyncFunc - the function to execute when the method is invoked.
     * @param transferReturnValue - an (optional) function that reports on which values in the `returnValue` should be transfered instead of copied.
     */
    addFunction(methodName, asyncFunc, transferReturnValue) {
        this.addMethodInternal(methodName, asyncFunc, transferReturnValue);
    }
    /**
     * Registers a function call for cross-thread invocation.
     * @param methodName - the name of the function to use during invocations.
     * @param asyncFunc - the function to execute when the method is invoked.
     */
    addVoidFunction(methodName, asyncFunc) {
        this.addMethodInternal(methodName, asyncFunc);
    }
    /**
     * Registers a class method call for cross-thread invocation.
     * @param obj - the object on which to find the method.
     * @param methodName - the name of the method to use during invocations.
     * @param transferReturnValue - an (optional) function that reports on which values in the `returnValue` should be transfered instead of copied.
     */
    addMethod(obj, methodName, method, transferReturnValue) {
        this.addFunction(methodName, method.bind(obj), transferReturnValue);
    }
    /**
     * Registers a class method call for cross-thread invocation.
     * @param methodName - the name of the method to use during invocations.
     * @param obj - the object on which to find the method.
     */
    addVoidMethod(obj, methodName, method) {
        this.addVoidFunction(methodName, method.bind(obj));
    }
    addEvent(object, eventName, makePayload, transferReturnValue) {
        object.addEventListener(eventName, (evt) => {
            let message = null;
            if (isDefined(makePayload)) {
                message = {
                    type: "event",
                    eventName,
                    data: makePayload(evt)
                };
            }
            else {
                message = {
                    type: "event",
                    eventName
                };
            }
            if (message.data !== undefined
                && isDefined(transferReturnValue)) {
                const transferables = transferReturnValue(message.data);
                this.postMessage(message, transferables);
            }
            else {
                this.postMessage(message);
            }
        });
    }
}
//# sourceMappingURL=WorkerServer.js.map