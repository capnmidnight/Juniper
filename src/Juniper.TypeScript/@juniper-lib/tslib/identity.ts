export function identity<T>(item: T): T {
    return item;
}

export function nothing(): void {
}

export function negate(value: number) {
    return -value;
}

export function alwaysTrue(): true {
    return true;
}

export function alwaysFalse(): false {
    return false;
}

export function not(value: boolean) {
    return !value;
}

export function and(a: boolean, b: boolean): boolean {
    return a && b;
}

export function or(a: boolean, b: boolean): boolean {
    return a || b;
}

export function xor(a: boolean, b: boolean): boolean {
    return a !== b;
}

export function nand(a: boolean, b: boolean): boolean {
    return not(and(a, b));
}

export function nor(a: boolean, b: boolean): boolean {
    return not(or(a, b));
}

export function equal<T>(a: T, b: T): boolean {
    return a === b;
}

export function reflectValue<T>(v: T): () => T {
    return () => v;
}

export type AsyncCallback = () => Promise<void>;