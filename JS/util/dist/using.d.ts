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
export declare function isDisposable(obj: any): obj is IDisposable;
export declare function isDestroyable(obj: any): obj is IDestroyable;
export declare function isClosable(obj: any): obj is IClosable;
export declare function isCloneable(obj: any): obj is ICloneable;
export declare function dispose(val: any): void;
type Cleanupable = IDisposable | IClosable | IDestroyable;
export declare function using<T extends Cleanupable, U>(val: T, thunk: (val: T) => U): U;
export declare function usingArray<T extends Cleanupable, U>(vals: T[], thunk: (val: T[]) => U): U;
export declare function usingAsync<T extends Cleanupable, U>(val: T, thunk: (val: T) => Promise<U>): Promise<U>;
export declare function usingArrayAsync<T extends Cleanupable, U>(vals: T[], thunk: (val: T[]) => Promise<U>): Promise<U>;
declare class TrashHeap implements IDisposable {
    #private;
    constructor(objs: IDisposable[]);
    dispose(): void;
    add(obj: IDisposable): void;
}
export declare function trashHeap(...objs: IDisposable[]): TrashHeap;
export {};
//# sourceMappingURL=using.d.ts.map