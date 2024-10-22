import { isString } from "@juniper-lib/util";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isRelationshipTypeModel(obj) {
    return isTypeStamped("relationshipType", obj)
        && isINamed(obj)
        && "parentRole" in obj
        && isString(obj.parentRole)
        && "childRole" in obj
        && isString(obj.childRole);
}
export function RELATIONSHIP_TYPE(idOrName) {
    return TypedNameOrId(idOrName, "relationshipType");
}
//# sourceMappingURL=RelationshipTypeModel.js.map