import { IClassificationMarked, isIClassificationMarked } from "./IClassificationMarked";
import { INamed, isINamed } from "./INamed";
import { ITypeStamped, isTypeStamped } from "./ITypeStamped";

export interface UserModel extends IClassificationMarked, INamed, ITypeStamped<"user"> {
}

export function isUserModel(obj: unknown): obj is UserModel {
    return isTypeStamped("user", obj)
        && isIClassificationMarked(obj)
        && isINamed(obj)

}