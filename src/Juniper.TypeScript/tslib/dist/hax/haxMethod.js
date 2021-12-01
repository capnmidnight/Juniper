import { Exception } from "../";
export function haxMethod(obj, name, method, hax, obj2 = null) {
    if (obj[name] !== method) {
        throw new Exception("Provided method is not the right " + name);
    }
    obj[name] = function (...params) {
        method.apply(obj, params);
        hax.apply(obj2, params);
    };
}
