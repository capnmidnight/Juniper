import { copyProps } from "./copyProps";
import { Hax } from "./Hax";


export function haxFunction<T, K extends keyof T & string, V extends T[K] & Function>(obj: T, method: V, name: K, hax: V) {
    if (method != obj[name]) {
        throw new Error(`The provided method is not the same object as the field "${name}" in the provided object.`);
    }

    (obj as any)[name] = function(...params: any[]) {
        hax(...params);
        return method.apply(this, params);
    };

    copyProps(method, obj[name]);

    return new Hax(obj, name, method);
}
