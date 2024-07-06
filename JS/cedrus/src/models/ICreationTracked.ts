import { isDate, isObject } from "@juniper-lib/util";
import { UserModel, isUserModel } from "./UserModel";

export interface ICreationTracked {
    createdOn: Date;
    user: UserModel;
}

export function isICreationTracked(obj: unknown): obj is ICreationTracked {
    return isObject(obj)
        && "createdOn" in obj
        && isDate(obj.createdOn)
        && "user" in obj
        && isUserModel(obj.user);
}