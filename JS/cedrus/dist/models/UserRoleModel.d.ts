import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { RoleModel } from "./RoleModel";
import { UserModel } from "./UserModel";
export interface UserRoleModel extends ITypeStamped<"userRole"> {
    user: UserModel;
    role: RoleModel;
}
export declare function isUserRoleModel(obj: unknown): obj is UserRoleModel;
export type USER_ROLE = TypedNameOrId<"userRole">;
export declare function USER_ROLE(idOrName: number | string): USER_ROLE;
//# sourceMappingURL=UserRoleModel.d.ts.map