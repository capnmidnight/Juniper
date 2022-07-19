import { BaseProgress, isArray, isDefined, TypedEventBase } from "@juniper-lib/tslib";
import { WorkerClientMethodCallMessage, WorkerServerErrorMessage, WorkerServerEventMessage, WorkerServerMessages, WorkerServerProgressMessage, WorkerServerReturnMessage } from "@juniper-lib/workers/WorkerMessages";

type workerServerMethod = (taskID: number, ...params: any[]) => Promise<void>;

type createTransferableCallback<T> = (returnValue: T) => (Transferable | OffscreenCanvas)[];

type Executor<T> = (...params: any[]) => Promise<T>;

type VoidExecutor = (...params: any[]) => void;

class WorkerServerProgress extends BaseProgress {
    constructor(private readonly server: WorkerServer<any>, private readonly taskID: number) {
        super();
    }


    /**
     * Report progress through long-running invocations. If your invocable
     * functions don't report progress, this can be safely ignored.
     * @param soFar - how much of the process we've gone through.
     * @param total - the total amount we need to go through.
     * @param msg - an optional message to include as part of the progress update.
     * @param est - an optional estimate of how many milliseconds are left in the progress.
     */
    override report(soFar: number, total: number, msg?: string, est?: number): void {
        const message: WorkerServerProgressMessage = {
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

export class WorkerServer<EventsT> {
    private methods = new Map<string, workerServerMethod>();

    /**
     * Creates a new worker thread method call listener.
     * @param self - the worker scope in which to listen.
     */
    constructor(private self: DedicatedWorkerGlobalScope) {
        this.self.addEventListener("message", (evt: MessageEvent<WorkerClientMethodCallMessage>): void => {
            const data = evt.data;
            this.callMethod(data);
        });
    }

    postMessage(message: WorkerServerMessages<EventsT>, transferables?: (Transferable | OffscreenCanvas)[]): void {
        if (isDefined(transferables)) {
            this.self.postMessage(message, transferables);
        }
        else {
            this.self.postMessage(message);
        }
    }

    private callMethod(data: WorkerClientMethodCallMessage) {
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
    private onError(taskID: number, errorMessage: string): void {
        const message: WorkerServerErrorMessage = {
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
    private onReturn<T>(taskID: number, returnValue: T, transferReturnValue: createTransferableCallback<T>): void {
        let message: WorkerServerReturnMessage = null;
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

    private addMethodInternal<T>(methodName: string, asyncFunc: Function, transferReturnValue?: createTransferableCallback<T>) {
        if (this.methods.has(methodName)) {
            throw new Error(`${methodName} method has already been mapped.`);
        }

        this.methods.set(methodName, async (taskID: number, ...params: any[]) => {
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
    addFunction<T>(methodName: string, asyncFunc: Executor<T>, transferReturnValue?: createTransferableCallback<T>) {
        this.addMethodInternal<T>(methodName, asyncFunc, transferReturnValue);
    }

    /**
     * Registers a function call for cross-thread invocation.
     * @param methodName - the name of the function to use during invocations.
     * @param asyncFunc - the function to execute when the method is invoked.
     */
    addVoidFunction(methodName: string, asyncFunc: VoidExecutor) {
        this.addMethodInternal(methodName, asyncFunc);
    }

    /**
     * Registers a class method call for cross-thread invocation.
     * @param obj - the object on which to find the method.
     * @param methodName - the name of the method to use during invocations.
     * @param transferReturnValue - an (optional) function that reports on which values in the `returnValue` should be transfered instead of copied.
     */
    addMethod<
        ClassT,
        MethodNameT extends keyof ClassT & string,
        MethodT extends ClassT[MethodNameT] & Executor<any>,
        ReturnT extends (ReturnType<MethodT> extends Promise<infer T> ? T : ReturnT)
    >(
        obj: ClassT,
        methodName: MethodNameT,
        method: MethodT,
        transferReturnValue?: createTransferableCallback<ReturnT>
    ): void {
        this.addFunction(methodName, method.bind(obj), transferReturnValue);
    }


    /**
     * Registers a class method call for cross-thread invocation.
     * @param methodName - the name of the method to use during invocations.
     * @param obj - the object on which to find the method.
     */
    addVoidMethod<
        ClassT,
        MethodNameT extends keyof ClassT & string,
        MethodT extends ClassT[MethodNameT] & VoidExecutor
    >(
        obj: ClassT,
        methodName: MethodNameT,
        method: MethodT
    ): void {
        this.addVoidFunction(methodName, method.bind(obj));
    }


    addEvent<TransferableT>(
        object: TypedEventBase<EventsT>,
        eventName: keyof EventsT & string,
        makePayload?: (evt: Event) => TransferableT,
        transferReturnValue?: createTransferableCallback<TransferableT>
    ): void {
        object.addEventListener(eventName, (evt: Event) => {
            let message: WorkerServerEventMessage<EventsT> = null;
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