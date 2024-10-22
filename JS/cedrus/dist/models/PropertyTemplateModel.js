import { isArray } from "@juniper-lib/util";
import { isEntityTypeModel } from "./EntityTypeModel";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isPropertyTemplateModel(obj) {
    return isTypeStamped("propertyTemplate", obj)
        && isINamed(obj)
        && "entityType" in obj
        && isEntityTypeModel(obj.entityType)
        && "propertyTypes" in obj
        && isArray(obj.propertyTypes);
}
export function PROPERTY_TEMPLATE(idOrName) {
    return TypedNameOrId(idOrName, "propertyTemplate");
}
//# sourceMappingURL=PropertyTemplateModel.js.map