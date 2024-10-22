import { isBoolean } from "@juniper-lib/util";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isEntityTypeModel(obj) {
    return isTypeStamped("entityType", obj)
        && isINamed(obj)
        && "isPrimary" in obj
        && isBoolean(obj.isPrimary);
}
export function ENTITY_TYPE(idOrName) {
    return TypedNameOrId(idOrName, "entityType");
}
//# sourceMappingURL=EntityTypeModel.js.map