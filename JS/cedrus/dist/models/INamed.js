import { isString } from "@juniper-lib/util";
import { isISequenced } from "./ISequenced";
export function isINamed(obj) {
    return isISequenced(obj)
        && "name" in obj
        && isString(obj.name);
}
export const getName = v => v?.name;
//# sourceMappingURL=INamed.js.map