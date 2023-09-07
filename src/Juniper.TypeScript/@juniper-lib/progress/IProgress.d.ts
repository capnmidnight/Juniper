export interface IProgress {
    report(soFar: number, total: number, message?: string, est?: number): void;
    attach(prog: IProgress): void;
    clear(): void;
    start(msg?: string): void;
    end(msg?: string): void;
}
export declare function isProgressCallback(obj: any): obj is IProgress;
//# sourceMappingURL=IProgress.d.ts.map