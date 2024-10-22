import { isTypeStamped, ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { isRoleModel, RoleModel } from "./RoleModel";
import { isUserModel, UserModel } from "./UserModel";

export interface UserRoleModel extends ITypeStamped<"userRole"> {
    user: UserModel;
    role: RoleModel;
}


export function isUserRoleModel(obj: unknown): obj is UserRoleModel {
    return isTypeStamped("userRole", obj)
        && "user" in obj
        && isUserModel(obj.user)
        && "role" in obj
        && isRoleModel(obj.role);
}

export type USER_ROLE = TypedNameOrId<"userRole">;
export function USER_ROLE(idOrName: number | string): USER_ROLE {
    return TypedNameOrId(idOrName, "userRole");
}