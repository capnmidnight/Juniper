import { isDate, isObject } from "@juniper-lib/util";
import { isUserModel } from "./UserModel";
export function isICreationTracked(obj) {
    return isObject(obj)
        && "createdOn" in obj
        && isDate(obj.createdOn)
        && "user" in obj
        && isUserModel(obj.user);
}
//# sourceMappingURL=ICreationTracked.js.map