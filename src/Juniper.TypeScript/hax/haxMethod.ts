export function haxMethod<T, K extends keyof T, V extends T[K] & Function>(obj: T, method: V, name: K, hax: V, obj2: any = null) {
    if (method != obj[name]) {
        throw new Error(`The provided method is not the same object as the field "${name}" in the provided object.`);
    }

    (obj as any)[name] = function (...params: any[]) {
        hax.apply(obj2, params);
        method.apply(obj, params);
    };
}
