import { ElementChild } from "@juniper-lib/dom";
import { IDebugLogger } from "./models";
export declare function WindowLogger(...rest: ElementChild[]): WindowLoggerElement;
export declare class WindowLoggerElement extends HTMLElement implements IDebugLogger {
    private readonly workerFunctions;
    private readonly logs;
    private readonly rows;
    private readonly onKeyPress;
    private readonly grid;
    private workerCount;
    static install(): import("@juniper-lib/dom").ElementFactory<WindowLoggerElement>;
    constructor();
    connectedCallback(): void;
    disconnectedCallback(): void;
    toggle(): void;
    open(): void;
    close(): void;
    private render;
    log(id: string, ...values: any[]): void;
    delete(id: string): void;
    clear(): void;
    addWorker(name: string, worker: Worker): void;
    private workerClear;
    private workerDelete;
    private workerLog;
}
//# sourceMappingURL=WindowLogger.d.ts.map