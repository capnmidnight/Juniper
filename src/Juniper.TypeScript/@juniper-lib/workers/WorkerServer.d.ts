import { TypedEventTarget, TypedEventMap } from "@juniper-lib/events/TypedEventTarget";
import { WorkerServerMessages } from "@juniper-lib/workers/WorkerMessages";
type createTransferableCallback<T> = (returnValue: T) => (Transferable | OffscreenCanvas)[];
type Executor<T> = (...params: any[]) => Promise<T>;
type VoidExecutor = (...params: any[]) => void;
export declare class WorkerServer<EventMapT extends TypedEventMap<string>> {
    private self;
    private methods;
    /**
     * Creates a new worker thread method call listener.
     * @param self - the worker scope in which to listen.
     */
    constructor(self: DedicatedWorkerGlobalScope);
    postMessage(message: WorkerServerMessages<EventMapT>, transferables?: (Transferable | OffscreenCanvas)[]): void;
    private callMethod;
    /**
     * Report an error back to the calling thread.
     * @param taskID - the invocation ID of the method that errored.
     * @param errorMessage - what happened?
     */
    private onError;
    /**
     * Return back to the client.
     * @param taskID - the invocation ID of the method that is returning.
     * @param returnValue - the (optional) value to return.
     * @param transferReturnValue - a mapping function to extract any Transferable objects from the return value.
     */
    private onReturn;
    private addMethodInternal;
    /**
     * Registers a function call for cross-thread invocation.
     * @param methodName - the name of the function to use during invocations.
     * @param asyncFunc - the function to execute when the method is invoked.
     * @param transferReturnValue - an (optional) function that reports on which values in the `returnValue` should be transfered instead of copied.
     */
    addFunction<T>(methodName: string, asyncFunc: Executor<T>, transferReturnValue?: createTransferableCallback<T>): void;
    /**
     * Registers a function call for cross-thread invocation.
     * @param methodName - the name of the function to use during invocations.
     * @param asyncFunc - the function to execute when the method is invoked.
     */
    addVoidFunction(methodName: string, asyncFunc: VoidExecutor): void;
    /**
     * Registers a class method call for cross-thread invocation.
     * @param obj - the object on which to find the method.
     * @param methodName - the name of the method to use during invocations.
     * @param transferReturnValue - an (optional) function that reports on which values in the `returnValue` should be transfered instead of copied.
     */
    addMethod<ClassT, MethodNameT extends keyof ClassT & string, MethodT extends ClassT[MethodNameT] & Executor<any>, ReturnT extends (ReturnType<MethodT> extends Promise<infer T> ? T : ReturnT)>(obj: ClassT, methodName: MethodNameT, method: MethodT, transferReturnValue?: createTransferableCallback<ReturnT>): void;
    /**
     * Registers a class method call for cross-thread invocation.
     * @param methodName - the name of the method to use during invocations.
     * @param obj - the object on which to find the method.
     */
    addVoidMethod<ClassT, MethodNameT extends keyof ClassT & string, MethodT extends ClassT[MethodNameT] & VoidExecutor>(obj: ClassT, methodName: MethodNameT, method: MethodT): void;
    addEvent<EventNameT extends keyof EventMapT & string, TransferableT>(object: TypedEventTarget<EventMapT>, eventName: EventNameT, makePayload?: (evt: EventMapT[EventNameT] & Event) => TransferableT, transferReturnValue?: createTransferableCallback<TransferableT>): void;
}
export {};
//# sourceMappingURL=WorkerServer.d.ts.map