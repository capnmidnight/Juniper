import { isArray, isString } from "@juniper-lib/util";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isUserModel(obj) {
    return isTypeStamped("user", obj)
        && isINamed(obj)
        && "email" in obj
        && isString(obj.email)
        && "roles" in obj
        && isArray(obj.roles)
        && obj.roles.every(isString);
}
export function USER(idOrName) {
    return TypedNameOrId(idOrName, "user");
}
//# sourceMappingURL=UserModel.js.map