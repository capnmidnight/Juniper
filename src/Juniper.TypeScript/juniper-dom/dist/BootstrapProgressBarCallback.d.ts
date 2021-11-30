import { BaseProgress } from "juniper-tslib";
export declare class BootstrapProgressBarCallback extends BaseProgress {
    private readonly progressBar;
    private readonly showMessage;
    constructor(progressBar: HTMLElement, showMessage?: boolean);
    report(soFar: number, total: number, message?: string, estimate?: number): void;
}
