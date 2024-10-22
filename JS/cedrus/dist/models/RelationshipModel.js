import { isEntityModel } from "./EntityModel";
import { isICreationTracked } from "./ICreationTracked";
import { isISequenced } from "./ISequenced";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { isRelationshipTypeModel } from "./RelationshipTypeModel";
export function isRelationshipModel(obj) {
    return isTypeStamped("relationship", obj)
        && isISequenced(obj)
        && isICreationTracked(obj)
        && "type" in obj
        && isRelationshipTypeModel(obj.type)
        && "parent" in obj
        && isEntityModel(obj.parent)
        && "child" in obj
        && isEntityModel(obj.child);
}
export function RELATIONSHIP(id) {
    return TypedNameOrId(id, "relationship");
}
//# sourceMappingURL=RelationshipModel.js.map