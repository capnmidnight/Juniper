import { Hax } from "./Hax";
interface Constructor<T = object> {
    new (...args: any[]): T;
    prototype: T;
}
export declare function haxClass<T, K extends keyof T & string, V, C extends T[K] & Constructor<V>>(obj: T, constructor: C, name: K, hax: (...args: ConstructorParameters<C>) => any, obj2?: any): Hax<T, K, C>;
export {};
//# sourceMappingURL=haxClass.d.ts.map