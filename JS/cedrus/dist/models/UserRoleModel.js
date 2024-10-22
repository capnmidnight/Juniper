import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { isRoleModel } from "./RoleModel";
import { isUserModel } from "./UserModel";
export function isUserRoleModel(obj) {
    return isTypeStamped("userRole", obj)
        && "user" in obj
        && isUserModel(obj.user)
        && "role" in obj
        && isRoleModel(obj.role);
}
export function USER_ROLE(idOrName) {
    return TypedNameOrId(idOrName, "userRole");
}
//# sourceMappingURL=UserRoleModel.js.map