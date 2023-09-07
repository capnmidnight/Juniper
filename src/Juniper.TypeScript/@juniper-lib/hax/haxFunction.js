import { copyProps } from "./copyProps";
import { Hax } from "./Hax";
export function haxFunction(obj, method, name, hax) {
    if (method != obj[name]) {
        throw new Error(`The provided method is not the same object as the field "${name}" in the provided object.`);
    }
    obj[name] = function (...params) {
        hax(...params);
        return method.apply(this, params);
    };
    copyProps(method, obj[name]);
    return new Hax(obj, name, method);
}
//# sourceMappingURL=haxFunction.js.map