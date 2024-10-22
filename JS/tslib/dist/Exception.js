export class Exception extends Error {
    constructor(message, innerError = null) {
        super(message);
        this.innerError = innerError;
    }
}
//# sourceMappingURL=Exception.js.map