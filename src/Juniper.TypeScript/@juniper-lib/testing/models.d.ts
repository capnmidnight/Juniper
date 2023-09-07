export interface IDebugLogger {
    log(id: string, ...values: any[]): void;
    delete(id: string): void;
    clear(): void;
    addWorker(name: string, worker: Worker): void;
}
export type MessageType = "log" | "delete" | "clear";
export declare const KEY = "XXX_QUAKE_LOGGER_XXX";
export interface IWorkerLoggerMessageData {
    key: string;
    method: MessageType;
    id: string;
    values: any[];
}
export declare function isWorkerLoggerMessageData(data: any): data is IWorkerLoggerMessageData;
//# sourceMappingURL=models.d.ts.map