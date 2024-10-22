import { isArray } from "@juniper-lib/util";
import { isEntityTypeModel } from "./EntityTypeModel";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { isRelationshipTypeModel } from "./RelationshipTypeModel";
export function isRelationshipTemplateModel(obj) {
    return isTypeStamped("relationshipTemplate", obj)
        && isINamed(obj)
        && "entityType" in obj
        && isEntityTypeModel(obj.entityType)
        && "relationshipType" in obj
        && isRelationshipTypeModel(obj.relationshipType)
        && "entityTypes" in obj
        && isArray(obj.entityTypes)
        && obj.entityTypes.every(isEntityTypeModel);
}
export function RELATIONSHIP_TEMPLATE(idOrName) {
    return TypedNameOrId(idOrName, "relationshipTemplate");
}
//# sourceMappingURL=RelationshipTemplateModel.js.map