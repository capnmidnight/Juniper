import { copyProps } from "./copyProps";
import { Hax } from "./Hax";
export function haxMethod(obj, method, name, hax, obj2 = null) {
    if (method != obj[name]) {
        throw new Error(`The provided method is not the same object as the field "${name}" in the provided object.`);
    }
    obj[name] = function (...params) {
        hax.apply(obj2, params);
        return method.apply(obj, params);
    };
    copyProps(method, obj[name]);
    return new Hax(obj, name, method);
}
//# sourceMappingURL=haxMethod.js.map