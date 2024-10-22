export class Hax {
    constructor(source, key, value) {
        this.source = source;
        this.key = key;
        this.value = value;
        this.disposed = false;
    }
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.source[this.key] = this.value;
        }
    }
}
//# sourceMappingURL=Hax.js.map