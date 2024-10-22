import { isDefined } from "@juniper-lib/util";

export interface IDebugLogger {
    log(id: string, ...values: any[]): void;
    delete(id: string): void;
    clear(): void;
    addWorker(name: string, worker: Worker): void;
}

export type MessageType = "log" | "delete" | "clear";

export const KEY = /*@__PURE__*/ "XXX_QUAKE_LOGGER_XXX";

export interface IWorkerLoggerMessageData {
    key: string;
    method: MessageType;
    id: string;
    values: any[];
}

export function isWorkerLoggerMessageData(data: any): data is IWorkerLoggerMessageData {
    return isDefined(data)
        && "key" in data
        && data.key === KEY;
}