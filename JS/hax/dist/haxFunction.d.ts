import { Hax } from "./Hax";
export declare function haxFunction<T, K extends keyof T & string, V extends T[K] & Function>(obj: T, method: V, name: K, hax: V): Hax<T, K, V>;
//# sourceMappingURL=haxFunction.d.ts.map