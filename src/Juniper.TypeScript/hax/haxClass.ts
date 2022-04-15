import { copyProps } from "./copyProps";
import { Hax } from "./Hax";

interface Constructor<T = object> {
    new(...args: any[]): T;
    prototype: T;
}

export function haxClass<T, K extends keyof T, V, C extends T[K] & Constructor<V>>(obj: T, constructor: C, name: K, hax: (...args: ConstructorParameters<C>) => any, obj2: any = null) {
    if (constructor !== obj[name]) {
        throw new Error(`The provided class constructor is not the same object as the field "${name}" in the provided object.`);
    }

    (obj as any)[name] = function (...args: ConstructorParameters<C>) {
        hax.apply(obj2, args);
        return new constructor(...args);
    };

    copyProps(constructor, obj[name]);

    return new Hax(obj, name, constructor);
}