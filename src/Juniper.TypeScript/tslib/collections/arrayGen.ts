export function arrayGen<T>(count: number, thunk: (i: number) => T): T[] {
    return Array.from(iterableGen(count, thunk));
}

export function* iterableGen(count: number, thunk: (i: number) => T) {
    for (let i = 0; i < count; ++i) {
        yield thunk(i);
    }
}
