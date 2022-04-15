import { IDisposable } from "@juniper/tslib"

export class Hax<T, K extends keyof T, V extends T[K]> implements IDisposable {

    constructor(private readonly source: T, private readonly key: K, private readonly value: V) {
    }

    private disposed = false;

    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.source[this.key] = this.value;
        }
    }
}