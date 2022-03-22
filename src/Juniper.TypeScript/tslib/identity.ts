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

export function invert(value: boolean) {
    return !value;
}

export type AsyncCallback = () => Promise<void>;