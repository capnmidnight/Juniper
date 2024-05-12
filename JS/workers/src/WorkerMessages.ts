export type WorkerClientMessageType = "methodCall";

export interface WorkerClientMethodCallMessage {
    type: "methodCall";
    taskID: number;
    methodName: string;
    params?: any[];
}

export type WorkerServerMessageType = "error"
    | "progress"
    | "return"
    | "event";

interface WorkerServerMessage<T extends WorkerServerMessageType> {
    type: T;
}

interface WorkerServerTaskMessage<T extends WorkerServerMessageType>
    extends WorkerServerMessage<T> {
    taskID: number;
}

export interface WorkerServerEventMessage<EventMap>
    extends WorkerServerMessage<"event"> {
    eventName: keyof EventMap;
    data?: any;
}

export interface WorkerServerErrorMessage
    extends WorkerServerTaskMessage<"error"> {
    errorMessage: string;
}

export interface WorkerServerProgressMessage
    extends WorkerServerTaskMessage<"progress"> {
    soFar: number;
    total: number;
    msg: string;
    est: number;
}

export interface WorkerServerReturnMessage
    extends WorkerServerTaskMessage<"return"> {
    returnValue?: any;
}

export type WorkerServerMessages<EventMap> = WorkerServerErrorMessage
    | WorkerServerProgressMessage
    | WorkerServerReturnMessage
    | WorkerServerEventMessage<EventMap>;