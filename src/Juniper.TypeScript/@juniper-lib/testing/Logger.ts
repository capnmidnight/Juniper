import { ILogger } from "./models";
import { WindowLogger } from "./WindowLogger";
import { WorkerLogger } from "./WorkerLogger";

declare const IS_WORKER: boolean;
const G = globalThis as any;
const X = Symbol(1124198212012021);
const logger: ILogger = G[X] = (G[X] as ILogger
    || IS_WORKER && new WorkerLogger()
    || new WindowLogger());

export class Logger implements ILogger {

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

