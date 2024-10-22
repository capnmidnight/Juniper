import { arrayCompare } from "./arrays";
export class Exception extends Error {
    constructor(message, innerError = null) {
        super(message);
        this.innerError = innerError;
    }
}
export function assert(name, value, expected) {
    const areArrays = value instanceof Array && expected instanceof Array;
    if (!(areArrays && arrayCompare(value, expected) === -1
        || !areArrays && value === expected)) {
        throw new AssertFailedException(name, `Got ${value}. Expected ${expected}.`);
    }
}
export class AssertFailedException extends Exception {
    constructor(name, message) {
        super(message);
        this.#name = name;
        Object.freeze(this);
    }
    #name;
    get name() { return this.#name; }
}
export async function withErrorAlert(action) {
    try {
        await action();
    }
    catch (exp) {
        console.error(exp);
        if (exp instanceof Error || exp instanceof Exception) {
            const match = exp.message.match(/Reason: (.+)/m);
            const reason = match && match[1] || exp.message;
            alert(reason);
        }
        else {
            console.error(exp);
        }
    }
}
//# sourceMappingURL=Exception.js.map