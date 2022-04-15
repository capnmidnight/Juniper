import { BaseProgress, IProgress } from "@juniper/progress";

class ConsoleProgressCallback extends BaseProgress {
    constructor(private readonly name: string, private readonly prog: IProgress) {
        super();
    }

    override report(soFar: number, total: number, msg?: string, est?: number) {
        super.report(soFar, total, msg, est);

        console.log(this.name, (100 * this.p).toFixed(1), msg);
        if (this.prog) {
            this.prog.report(soFar, total, msg, est);
        }
    }
}

export function progressLogger(name: string, prog?: IProgress): IProgress {
    return new ConsoleProgressCallback(name, prog);
}
