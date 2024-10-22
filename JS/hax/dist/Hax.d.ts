import { IDisposable } from "@juniper-lib/tslib/dist/using";
export declare class Hax<T, K extends keyof T, V extends T[K]> implements IDisposable {
    private readonly source;
    private readonly key;
    private readonly value;
    constructor(source: T, key: K, value: V);
    private disposed;
    dispose(): void;
}
//# sourceMappingURL=Hax.d.ts.map