import { ErsatzElement } from "@juniper-lib/dom/tags";
import { BaseProgress } from "@juniper-lib/progress/BaseProgress";
import { IProgress } from "@juniper-lib/progress/IProgress";
export declare class BootstrapProgressBarElement implements ErsatzElement, IProgress {
    readonly element: HTMLElement;
    private readonly progress;
    constructor(outerClassName?: string);
    clear(): void;
    report(soFar: number, total: number, message?: string, est?: number): void;
    attach(prog: IProgress): void;
    start(msg?: string): void;
    end(msg?: string): void;
}
export declare class BootstrapProgressBarCallback extends BaseProgress {
    private readonly progressBar;
    private readonly showMessage;
    constructor(progressBar: HTMLElement, showMessage?: boolean);
    report(soFar: number, total: number, message?: string, estimate?: number): void;
}
export declare function BootstrapProgressBar(outerClassName?: string): BootstrapProgressBarElement;
//# sourceMappingURL=BootstrapProgressBar.d.ts.map