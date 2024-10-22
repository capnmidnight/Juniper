import { isArray, isString } from "@juniper-lib/util";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";

export interface UserModel extends INamed, ITypeStamped<"user"> {
    email: string;
    roles: string[];
}

export function isUserModel(obj: unknown): obj is UserModel {
    return isTypeStamped("user", obj)
        && isINamed(obj)
        && "email" in obj
        && isString(obj.email)
        && "roles" in obj
        && isArray(obj.roles)
        && obj.roles.every(isString);
}

export interface SetUserInput {
    userName: string;
    email: string;
}


export type USER = TypedNameOrId<"user">;
export function USER(idOrName: number | string): USER {
    return TypedNameOrId(idOrName, "user");
}