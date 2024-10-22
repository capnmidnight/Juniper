import { Query } from "@juniper-lib/dom";
import { IDebugLogger } from "./models";
import { WindowLogger } from "./WindowLogger";
import { WorkerLogger } from "./WorkerLogger";

declare const IS_WORKER: boolean;
const G = globalThis as any;
const X = Symbol(1124198212012021);
const logger: IDebugLogger = G[X] = (G[X] as IDebugLogger
    || IS_WORKER && new WorkerLogger()
    || WindowLogger(Query("window-logger")));

export class DebugLogger implements IDebugLogger {

    log(id: string, ...values: any[]): void {
        logger.log(id, ...values);
    }

    delete(id: string): void {
        logger.delete(id);
    }

    clear(): void {
        logger.clear();
    }

    addWorker(name: string, worker: Worker): void {
        logger.addWorker(name, worker);
    }
}

