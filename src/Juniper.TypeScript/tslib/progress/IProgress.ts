import { isDefined, isFunction } from "../typeChecks";

export interface IProgress {
    report(soFar: number, total: number, message?: string, est?: number): void;
    attach(prog: IProgress): void;
    end(): void;
}

export function isProgressCallback(obj: any): obj is IProgress {
    return isDefined(obj)
        && isFunction(obj.report)
        && isFunction(obj.attach)
        && isFunction(obj.end);
}