import { isDefined, isFunction, isObject } from "./typeChecks";

export interface IDisposable {
    dispose(): void;
}

export interface IClosable {
    close(): void;
}

export interface ICloneable {
    clone(): unknown;
}

export interface IDestroyable {
    destroy(): void;
}

function interfaceSigCheck<T>(obj: any, ...funcNames: string[]): obj is T {
    if (!isObject(obj)) {
        return false;
    }

    obj = obj as any;

    for (const funcName of funcNames) {
        if (!(funcName in obj)) {
            return false;
        }

        const func = obj[funcName];
        if (!isFunction(func)) {
            return false;
        }
    }

    return true;
}

export function isDisposable(obj: any): obj is IDisposable {
    return interfaceSigCheck(obj, "dispose");
}

export function isDestroyable(obj: any): obj is IDestroyable {
    return interfaceSigCheck(obj, "destroy");
}

export function isClosable(obj: any): obj is IClosable {
    return interfaceSigCheck(obj, "close");
}

export function isCloneable(obj: any): obj is ICloneable {
    return interfaceSigCheck(obj, "clone");
}

export function dispose(val: any): void {
    if (isDisposable(val)) {
        val.dispose();
    }

    if (isClosable(val)) {
        val.close();
    }

    if (isDestroyable(val)) {
        val.destroy();
    }
}

type Cleanupable = IDisposable | IClosable | IDestroyable;

export function using<T extends Cleanupable, U>(val: T, thunk: (val: T) => U): U {
    try {
        return thunk(val);
    } finally {
        dispose(val);
    }
}

export function usingArray<T extends Cleanupable, U>(vals: T[], thunk: (val: T[]) => U): U {
    try {
        return thunk(vals);
    } finally {
        if (isDefined(vals)) {
            for (const val of vals) {
                dispose(val);
            }
        }
    }
}

export async function usingAsync<T extends Cleanupable, U>(val: T, thunk: (val: T) => Promise<U>): Promise<U> {
    try {
        return await thunk(val);
    } finally {
        dispose(val);
    }
}


export async function usingArrayAsync<T extends Cleanupable, U>(vals: T[], thunk: (val: T[]) => Promise<U>): Promise<U> {
    try {
        return await thunk(vals);
    } finally {
        if (isDefined(vals)) {
            for (const val of vals) {
                dispose(val);
            }
        }
    }
}

class TrashHeap implements IDisposable {
    constructor(private readonly objs: IDisposable[]) {
    }

    dispose() {
        for (const obj of this.objs) {
            dispose(obj);
        }
    }

    add(obj: IDisposable) {
        this.objs.push(obj);
    }
}

export function trashHeap(...objs: IDisposable[]): TrashHeap {
    return new TrashHeap(objs);
}