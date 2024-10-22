import { arrayCompare } from "./arrays";

export class Exception extends Error {
    constructor(message: string, public readonly innerError: any = null) {
        super(message);
    }
}

export function assert(name: string, value: any, expected: any) {
    const areArrays = value instanceof Array && expected instanceof Array;

    if (!(areArrays && arrayCompare(value, expected) === -1
        || !areArrays && value === expected)) {
        throw new AssertFailedException(name, `Got ${value}. Expected ${expected}.`);
    }
}

export class AssertFailedException extends Exception {
    constructor(name: string, message: string) {
        super(message);
        this.#name = name;
        Object.freeze(this);
    }

    #name: string;
    override get name() { return this.#name; }
}

export async function withErrorAlert(action: () => (void | Promise<void>)) {
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
