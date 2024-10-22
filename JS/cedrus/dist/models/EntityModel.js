import { isDate, isNullOrUndefined, isObject, isString } from "@juniper-lib/util";
import { isEntityTypeModel } from "./EntityTypeModel";
import { isICreationTracked } from "./ICreationTracked";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { isUserModel } from "./UserModel";
export function isEntityModel(obj) {
    return isTypeStamped("entity", obj)
        && isINamed(obj)
        && isICreationTracked(obj)
        && "link" in obj
        && isString(obj.link)
        && "type" in obj
        && isEntityTypeModel(obj.type)
        && "reviewedBy" in obj
        && (isUserModel(obj.reviewedBy)
            || isObject(obj.reviewedBy)
                && "id" in obj.reviewedBy
                && isNullOrUndefined(obj.reviewedBy.id))
        && "reviewedOn" in obj
        && (isNullOrUndefined(obj.reviewedOn)
            || isDate(obj.reviewedOn));
}
export function ENTITY(idOrName) {
    return TypedNameOrId(idOrName, "entity");
}
//# sourceMappingURL=EntityModel.js.map