import { Exception } from "../";

export function haxMethod<T, K extends keyof T, V extends Function>(obj: T, name: K, method: V, hax: V, obj2: any = null) {
    if ((obj as any)[name] !== method) {
        throw new Exception("Provided method is not the right " + name);
    }

    (obj as any)[name] = function(...params: any[]) {
        method.apply(obj, params);
        hax.apply(obj2, params);
    };
}
