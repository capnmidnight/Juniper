import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isRoleModel(obj) {
    return isTypeStamped("role", obj)
        && isINamed(obj);
}
export function ROLE(idOrName) {
    return TypedNameOrId(idOrName, "role");
}
//# sourceMappingURL=RoleModel.js.map