import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export interface RoleModel extends INamed, ITypeStamped<"role"> {
}
export declare function isRoleModel(obj: unknown): obj is RoleModel;
export type ROLE = TypedNameOrId<"role">;
export declare function ROLE(idOrName: number | string): ROLE;
//# sourceMappingURL=RoleModel.d.ts.map