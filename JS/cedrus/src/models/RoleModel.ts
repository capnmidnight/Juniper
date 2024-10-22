import { INamed, isINamed } from "./INamed";
import { isTypeStamped, ITypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";

export interface RoleModel extends INamed, ITypeStamped<"role"> {
}

export function isRoleModel(obj: unknown): obj is RoleModel {
    return isTypeStamped("role", obj)
        && isINamed(obj);
}

export type ROLE = TypedNameOrId<"role">;
export function ROLE(idOrName: number | string): ROLE {
    return TypedNameOrId(idOrName, "role");
}