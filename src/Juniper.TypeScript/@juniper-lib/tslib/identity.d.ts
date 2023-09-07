export declare function identity<T>(item: T): T;
export declare function nothing(): void;
export declare function negate(value: number): number;
export declare function alwaysTrue(): true;
export declare function alwaysFalse(): false;
export declare function not(value: boolean): boolean;
export declare function and(a: boolean, b: boolean): boolean;
export declare function or(a: boolean, b: boolean): boolean;
export declare function xor(a: boolean, b: boolean): boolean;
export declare function nand(a: boolean, b: boolean): boolean;
export declare function nor(a: boolean, b: boolean): boolean;
export declare function equal<T>(a: T, b: T): boolean;
export declare function reflectValue<T>(v: T): () => T;
export type AsyncCallback = () => Promise<void>;
//# sourceMappingURL=identity.d.ts.map