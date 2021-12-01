import { isWorker } from "juniper-tslib";
import { WindowLogger } from "./WindowLogger";
import { WorkerLogger } from "./WorkerLogger";
const G = globalThis;
const X = Symbol(1124198212012021);
const logger = G[X] = (G[X]
    || isWorker && new WorkerLogger()
    || new WindowLogger());
export class Logger {
    log(id, ...values) {
        logger.log(id, ...values);
    }
    delete(id) {
        logger.delete(id);
    }
    clear() {
        logger.clear();
    }
    addWorker(name, worker) {
        logger.addWorker(name, worker);
    }
}
