import { BaseProgress, IProgress } from "@juniper/tslib";


export function progressHTML(prog: HTMLProgressElement): IProgress {
    return new HTMLProgressBarCallback(prog);
}

class HTMLProgressBarCallback extends BaseProgress {
    constructor(private readonly prog: HTMLProgressElement) {
        super();
    }

    override report(soFar: number, total: number, message?: string, estimate?: number) {
        super.report(soFar, total, message, estimate);
        this.prog.max = total;
        this.prog.value = soFar;
    }
}
