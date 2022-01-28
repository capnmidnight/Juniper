import { Exception } from "../";
import { isNullOrUndefined } from "../typeChecks";

export function haxMethod<T, K extends keyof T, V extends T[K] & Function>(obj: T, name: K, hax: V, obj2: any = null) {
    const method = obj[name] as V;
    if (isNullOrUndefined(method)) {
        throw new Exception(`There is no method named "${name}" in the provided object.`);
    }

    (obj as any)[name] = function(...params: any[]) {
        method.apply(obj, params);
        hax.apply(obj2, params);
    };
}
