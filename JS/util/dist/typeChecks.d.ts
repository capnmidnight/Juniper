export declare function isFunction(obj: any): obj is Function;
export declare function isString(obj: any): obj is string;
export declare function isBoolean(obj: any): obj is boolean;
export declare function isNumber(obj: any): obj is number;
/**
 * Check a value to see if it is of a number type
 * and is the special NaN value or one of the Infinities.
 */
export declare function isBadNumber(num: number): boolean;
/**
 * Check a value to see if it is of a number type
 * and is not the special NaN value nor one of the Infinities.
 */
export declare function isGoodNumber(obj: any): obj is number;
export declare function isObject(obj: any): obj is object;
export declare function isPromise<T>(obj: any): obj is Promise<T>;
export declare function isDate(obj: any): obj is Date;
export declare function isArray(obj: any): obj is Array<any>;
export declare function isSingleDimArray<T>(arr: T[] | T[][] | T[][][]): arr is T[];
export declare function assertNever(x: never, msg?: string): never;
export declare function isNullOrUndefined<T>(obj: T | null | undefined | void): obj is null | undefined | void;
export declare function isDefined<T>(obj: T | null | undefined | void): obj is T;
export declare function isEventListener(obj: EventListenerOrEventListenerObject): obj is EventListener;
export declare function isEventListenerObject(obj: EventListenerOrEventListenerObject): obj is EventListenerObject;
export declare function isArrayBufferView(obj: any): obj is ArrayBufferView;
export declare function isArrayBuffer(val: any): val is ArrayBuffer;
//# sourceMappingURL=typeChecks.d.ts.map