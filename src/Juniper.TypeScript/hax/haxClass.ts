import { copyProps } from "./copyProps";
import { Hax } from "./Hax";

interface Constructor<T = object> {
    new(...args: any[]): T;
    prototype: T;
}

export function haxClass<S, K extends keyof S, V, T extends S[K] & Constructor<V>>(obj: S, constructor: T, name: K, hax: (...args: ConstructorParameters<T>) => any) {
    if (constructor !== obj[name]) {
        throw new Error(`The provided class constructor is not the same object as the field "${name}" in the provided object.`);
    }

    (obj as any)[name] = function (...args: ConstructorParameters<T>) {
        hax(...args);
        return new constructor(...args);
    };

    copyProps(constructor, obj[name]);

    return new Hax(obj, name, constructor);
}