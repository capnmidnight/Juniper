import { ITypeStamped } from "./ITypeStamped";
import { RoleModel } from "./RoleModel";
import { UserModel } from "./UserModel";

export interface UserRoleModel extends ITypeStamped<"userRole"> {
    user: UserModel;
    role: RoleModel;
}
