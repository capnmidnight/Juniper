import { Query } from "@juniper-lib/dom";
import { WindowLogger } from "./WindowLogger";
import { WorkerLogger } from "./WorkerLogger";
const G = globalThis;
const X = Symbol(1124198212012021);
const logger = G[X] = (G[X]
    || IS_WORKER && new WorkerLogger()
    || WindowLogger(Query("window-logger")));
export class DebugLogger {
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
//# sourceMappingURL=DebugLogger.js.map