import { Hax } from "./Hax";
export declare function haxMethod<T, K extends keyof T & string, V extends T[K] & Function>(obj: T, method: V, name: K, hax: V, obj2?: any): Hax<T, K, V>;
//# sourceMappingURL=haxMethod.d.ts.map