import { isString } from "@juniper-lib/util";
import { hasDataType, hasStorageType, isDataType, isStorageType } from "./DataType";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isPropertyTypeModel(obj, dataOrStorageType, storageType) {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;
    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;
    return isTypeStamped("propertyType", obj)
        && hasDataType(obj, dataType)
        && hasStorageType(obj, storageType)
        && "unitsCategory" in obj
        && isString(obj.unitsCategory)
        && "description" in obj
        && isString(obj.description);
}
export function PROPERTY_TYPE(idOrName) {
    return TypedNameOrId(idOrName, "propertyType");
}
//# sourceMappingURL=PropertyTypeModel.js.map