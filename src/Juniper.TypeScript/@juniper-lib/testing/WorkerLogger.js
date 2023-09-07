import { KEY } from "./models";
export class WorkerLogger {
    constructor() {
        this.msg = {
            key: KEY,
            method: null,
            id: null,
            values: null
        };
    }
    post(method, id, ...values) {
        this.msg.method = method;
        this.msg.id = id;
        this.msg.values = values;
        self.postMessage(this.msg);
    }
    log(id, ...values) {
        this.post("log", id, ...values);
    }
    delete(id) {
        this.post("delete", id);
    }
    clear() {
        this.post("clear");
    }
    addWorker(_name, _worker) {
        // no-op
    }
}
//# sourceMappingURL=WorkerLogger.js.map