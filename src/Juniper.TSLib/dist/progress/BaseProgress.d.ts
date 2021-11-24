import type { IProgress } from "./IProgress";
export declare abstract class BaseProgress implements IProgress {
    private readonly attached;
    private soFar;
    private total;
    private msg;
    private est;
    report(soFar: number, total: number, msg?: string, est?: number): void;
    attach(prog: IProgress): void;
    end(): void;
}
