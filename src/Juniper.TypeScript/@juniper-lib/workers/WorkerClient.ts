import { arrayScan } from "@juniper-lib/collections/arrays";
import { Task } from "@juniper-lib/events/Task";
import { TypedEventBase, TypedEventMap } from "@juniper-lib/events/TypedEventBase";
import { IProgress, isProgressCallback } from "@juniper-lib/progress/IProgress";
import { isWorkerSupported } from "@juniper-lib/tslib/flags";
import { assertNever, isArray, isDefined } from "@juniper-lib/tslib/typeChecks";
import type { IDisposable } from "@juniper-lib/tslib/using";
import type {
    WorkerClientMethodCallMessage,
    WorkerServerErrorMessage,
    WorkerServerEventMessage,
    WorkerServerMessages,
    WorkerServerProgressMessage,
    WorkerServerReturnMessage
} from "@juniper-lib/workers/WorkerMessages";

interface WorkerInvocation {
    prog: IProgress;
    task: Task<any>;
    methodName: string;
}

export abstract class WorkerClient<EventsMapT extends TypedEventMap<string> = TypedEventMap<string>> extends TypedEventBase<EventsMapT> implements IDisposable {
    private readonly invocations = new Map<number, WorkerInvocation>();
    private readonly tasks = new Array<Task<any>>();

    private taskCounter = 0;

    /**
     * Creates a new pooled worker method executor.
     * @param options
     */
    constructor(private worker: Worker) {
        super();

        if (!isWorkerSupported) {
            console.warn("Workers are not supported on this system.");
        }

        this.worker.addEventListener("message", (evt: MessageEvent<WorkerServerMessages<EventsMapT>>) => {
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

    private postMessage(message: WorkerClientMethodCallMessage, transferables?: (Transferable | OffscreenCanvas)[]) {
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

    protected abstract propogateEvent(data: WorkerServerEventMessage<EventsMapT>): void;

    private progressReport(data: WorkerServerProgressMessage) {
        const invocation = this.invocations.get(data.taskID);
        if (invocation) {
            const { prog } = invocation;
            if (prog) {
                prog.report(data.soFar, data.total, data.msg, data.est);
            }
        }
    }

    private methodReturned(data: WorkerServerReturnMessage) {
        const messageHandler = this.removeInvocation(data.taskID);
        const { task } = messageHandler;
        task.resolve(data.returnValue);
    }

    private invocationError(data: WorkerServerErrorMessage) {
        const messageHandler = this.removeInvocation(data.taskID);
        const { task, methodName } = messageHandler;
        task.reject(new Error(`${methodName} failed. Reason: ${data.errorMessage}`));
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
    callMethod<T>(methodName: string, params: any[], transferables: (Transferable | OffscreenCanvas)[], prog?: IProgress): Promise<T>;

    /**
     * Execute a method on a round-robin selected worker thread.
     * @param methodName - the name of the method to execute.
     * @param parameters - the parameters to pass to the method.
     * @param transferables - any values in any of the parameters that should be transfered instead of copied to the worker thread.
     * @param prog - a callback for receiving progress reports on long-running invocations.
     */
    callMethod<T>(methodName: string, parameters?: any[] | IProgress, transferables?: (Transferable | OffscreenCanvas)[] | IProgress, prog?: IProgress): Promise<T | undefined> {
        if (!isWorkerSupported) {
            return Promise.reject(new Error("Workers are not supported on this system."));
        }

        // Normalize method parameters.
        let params: any[] = null;
        let tfers: (Transferable | OffscreenCanvas)[] = null;

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
        let task = arrayScan(this.tasks, t => t.finished) as Task<T>;
        if (task) {
            task.reset();
        }
        else {
            task = new Task<T>();
            this.tasks.push(task);
        }
        const invocation: WorkerInvocation = {
            methodName,
            task,
            prog
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
