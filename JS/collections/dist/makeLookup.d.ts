export declare function makeLookup<T, U>(items: readonly T[], makeID: (item: T, i?: number) => U): Map<U, T>;
export declare function makeReverseLookup<T>(items: readonly T[]): Map<T, number>;
export declare function makeReverseLookup<T, U>(items: readonly T[], makeID: (item: T, i?: number) => U): Map<T, U>;
//# sourceMappingURL=makeLookup.d.ts.map