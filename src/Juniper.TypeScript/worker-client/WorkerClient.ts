import { Task } from "juniper-tslib";
import { TypedEvent, TypedEventBase } from "juniper-tslib/events/EventBase";
import type { IProgress } from "juniper-tslib/progress/IProgress";
import { isProgressCallback } from "juniper-tslib/progress/IProgress";
import { assertNever, isArray, isDefined } from "juniper-tslib/typeChecks";
import type { IDisposable } from "juniper-tslib/using";
import type {
    WorkerClientMethodCallMessage,
    WorkerServerErrorMessage,
    WorkerServerEventMessage,
    WorkerServerMessages,
    WorkerServerProgressMessage,
    WorkerServerReturnMessage
} from "juniper-workers/WorkerMessages";

interface WorkerInvocation {
    prog: IProgress;
    resolve: (value: any) => void;
    reject: (reason?: any) => void;
    methodName: string;
}

export class WorkerClient<EventsT> extends TypedEventBase<EventsT> implements IDisposable {
    static isSupported = "Worker" in globalThis;

    private taskCounter = 0;
    private invocations = new Map<number, WorkerInvocation>();

    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(private worker: Worker) {
        super();

        if (!WorkerClient.isSupported) {
            console.warn("Workers are not supported on this system.");
        }

        this.worker.addEventListener("message", (evt: MessageEvent<WorkerServerMessages>) => {
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

    private postMessage(message: WorkerClientMethodCallMessage, transferables?: Transferable[]) {
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

    private propogateEvent(data: WorkerServerEventMessage) {
        const evt = new TypedEvent(data.eventName);
        this.dispatchEvent(Object.assign(evt, data.data));
    }

    private progressReport(data: WorkerServerProgressMessage) {
        const invocation = this.invocations.get(data.taskID);
        const { prog } = invocation;
        if (prog) {
            prog.report(data.soFar, data.total, data.msg, data.est);
        }
    }

    private methodReturned(data: WorkerServerReturnMessage) {
        const messageHandler = this.removeInvocation(data.taskID);
        const { resolve } = messageHandler;
        resolve(data.returnValue);
    }

    private invocationError(data: WorkerServerErrorMessage) {
        const messageHandler = this.removeInvocation(data.taskID);
        const { reject, methodName } = messageHandler;
        reject(new Error(`${methodName} failed. Reason: ${data.errorMessage}`));
    }

    /**
     * When the invocation has errored, we want to stop listening to the worker
     * message channel so we don't eat up processing messages that have no chance
     * ever pertaining to the invocation.
     **/
    private removeInvocation(taskID: number) {
        const invocation = this.invocations.get(taskID);
        this.invocations.delete(taskID);
        return invocation;
    }

    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, prog?: IProgress): Promise<T>;

    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, params: any[], prog?: IProgress): Promise<T>;

    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param params - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, params: any[], transferables: Transferable[], prog?: IProgress): Promise<T>;

    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param parameters - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, parameters?: any[] | IProgress, transferables?: Transferable[] | IProgress, prog?: IProgress): Promise<T | undefined> {
        if (!WorkerClient.isSupported) {
            return Promise.reject(new Error("Workers are not supported on this system."));
        }

        // Normalize method parameters.
        let params: any[] = null;
        let tfers: Transferable[] = null;

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
        const taskID = this.taskCounter++;
        const task = new Task<T>();
        const invocation: WorkerInvocation = {
            prog,
            resolve: task.resolve,
            reject: task.reject,
            methodName
        };

        this.invocations.set(taskID, invocation);

        let message: WorkerClientMethodCallMessage = null;
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
        return task;
    }
}
