import { BaseProgress } from "./BaseProgress";
import { IProgress } from "./IProgress";

class ConsoleProgressCallback extends BaseProgress {

    readonly #name: string;
    readonly #prog: IProgress;

    constructor(name: string, prog: IProgress) {
        super();
        this.#name = name;
        this.#prog = prog;
    }

    override report(soFar: number, total: number, msg?: string, est?: number) {
        super.report(soFar, total, msg, est);

        console.log(this.#name, (100 * this.p).toFixed(1), msg);
        if (this.#prog) {
            this.#prog.report(soFar, total, msg, est);
        }
    }
}

export function progressLogger(name: string, prog?: IProgress): IProgress {
    return new ConsoleProgressCallback(name, prog);
}
