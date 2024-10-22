import { isObject } from "@juniper-lib/util";
import { hasDataType, hasStorageType, isDataType, isStorageType } from "./DataType";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
export function isPropertyTypeValidValueModel(obj, dataOrStorageType, storageType) {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;
    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;
    return isTypeStamped("propertyTypeValidValue", obj)
        && "propertyType" in obj
        && isObject(obj.propertyType)
        && hasDataType(obj.propertyType, dataType)
        && hasStorageType(obj.propertyType, storageType);
}
export function PROPERTY_TYPE_VALID_VALUE(idOrName) {
    return TypedNameOrId(idOrName, "propertyTypeValidValue");
}
//# sourceMappingURL=PropertyTypeValidValueModel.js.map