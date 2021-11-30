export interface IProgress {
    report(soFar: number, total: number, message?: string, est?: number): void;
    attach(prog: IProgress): void;
    end(): void;
}
export declare function isProgressCallback(obj: any): obj is IProgress;
