import { IDebugLogger } from "./models";
export declare class DebugLogger implements IDebugLogger {
    log(id: string, ...values: any[]): void;
    delete(id: string): void;
    clear(): void;
    addWorker(name: string, worker: Worker): void;
}
//# sourceMappingURL=DebugLogger.d.ts.map