import { isDefined, isFunction } from "../";

export interface IProgress {
    report(soFar: number, total: number, message?: string, est?: number): void;
    attach(prog: IProgress): void;
    clear(): void;
    start(msg?: string): void;
    end(msg?: string): void;
}

export function isProgressCallback(obj: any): obj is IProgress {
    return isDefined(obj)
        && isFunction(obj.report)
        && isFunction(obj.attach)
        && isFunction(obj.end);
}