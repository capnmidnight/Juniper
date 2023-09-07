import { copyProps } from "./copyProps";
import { Hax } from "./Hax";
export function haxClass(obj, constructor, name, hax, obj2 = null) {
    if (constructor !== obj[name]) {
        throw new Error(`The provided class constructor is not the same object as the field "${name}" in the provided object.`);
    }
    obj[name] = function (...args) {
        hax.apply(obj2, args);
        return new constructor(...args);
    };
    copyProps(constructor, obj[name]);
    return new Hax(obj, name, constructor);
}
//# sourceMappingURL=haxClass.js.map