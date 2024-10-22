import { IProgress } from "./IProgress";
export declare function progressOfArray<T, U>(prog: IProgress, items: T[], callback: (val: T, prog: IProgress, i?: number) => Promise<U>): Promise<U[]>;
//# sourceMappingURL=progressOfArray.d.ts.map