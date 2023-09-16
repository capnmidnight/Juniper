export declare function all<T extends readonly unknown[] | []>(...tasks: T): Promise<{
    -readonly [P in keyof T]: Awaited<T[P]>;
}>;
//# sourceMappingURL=all.d.ts.map