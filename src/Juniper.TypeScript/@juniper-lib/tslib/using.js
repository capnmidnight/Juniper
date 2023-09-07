import { isDefined, isFunction, isObject } from "./typeChecks";
function interfaceSigCheck(obj, ...funcNames) {
    if (!isObject(obj)) {
        return false;
    }
    obj = obj;
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
export function isDisposable(obj) {
    return interfaceSigCheck(obj, "dispose");
}
export function isDestroyable(obj) {
    return interfaceSigCheck(obj, "destroy");
}
export function isClosable(obj) {
    return interfaceSigCheck(obj, "close");
}
export function isCloneable(obj) {
    return interfaceSigCheck(obj, "clone");
}
export function dispose(val) {
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
export function using(val, thunk) {
    try {
        return thunk(val);
    }
    finally {
        dispose(val);
    }
}
export function usingArray(vals, thunk) {
    try {
        return thunk(vals);
    }
    finally {
        if (isDefined(vals)) {
            for (const val of vals) {
                dispose(val);
            }
        }
    }
}
export async function usingAsync(val, thunk) {
    try {
        return await thunk(val);
    }
    finally {
        dispose(val);
    }
}
export async function usingArrayAsync(vals, thunk) {
    try {
        return await thunk(vals);
    }
    finally {
        if (isDefined(vals)) {
            for (const val of vals) {
                dispose(val);
            }
        }
    }
}
class TrashHeap {
    constructor(objs) {
        this.objs = objs;
    }
    dispose() {
        for (const obj of this.objs) {
            dispose(obj);
        }
    }
    add(obj) {
        this.objs.push(obj);
    }
}
export function trashHeap(...objs) {
    return new TrashHeap(objs);
}
//# sourceMappingURL=using.js.map