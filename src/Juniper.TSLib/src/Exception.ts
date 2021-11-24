export class Exception extends Error {
    constructor(message: string, public readonly innerError: any = null) {
        super(message);
    }
}