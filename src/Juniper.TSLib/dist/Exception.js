export class Exception extends Error {
    innerError;
    constructor(message, innerError = null) {
        super(message);
        this.innerError = innerError;
    }
}
