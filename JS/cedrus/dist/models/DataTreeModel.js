import { isArray, isBoolean, isNumber, isString } from "@juniper-lib/util";
import { hasDataType, hasStorageType, isDataType, isStorageType } from "./DataType";
import { isINamed } from "./INamed";
import { isISequenced } from "./ISequenced";
export function isFlatEntityTypeModel(obj) {
    return isINamed(obj)
        && "isPrimary" in obj
        && isBoolean(obj.isPrimary)
        && (!("parentId" in obj)
            || obj.parentId === null
            || isNumber(obj.parentId));
}
export function isFlatEntityModel(obj) {
    return isINamed(obj)
        && "typeId" in obj
        && isNumber(obj.typeId)
        && "properties" in obj
        && isArray(obj.properties)
        && obj.properties.every(isNumber)
        && "parents" in obj
        && isArray(obj.parents)
        && obj.parents.every(isFlatRelationshipModel)
        && "children" in obj
        && isArray(obj.children)
        && obj.children.every(isFlatRelationshipModel);
}
export function isFlatPropertyModel(obj, dataOrStorageType, storageType) {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;
    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;
    return isINamed(obj)
        && "typeId" in obj
        && isNumber(obj.typeId)
        && "description" in obj
        && isString(obj.description)
        && hasDataType(obj, dataType)
        && hasStorageType(obj, storageType)
        && "unitsCategory" in obj
        && isString(obj.unitsCategory)
        && "units" in obj
        && isString(obj.units)
        && "value" in obj;
}
export function isFlatRelationshipModel(obj) {
    return isISequenced(obj);
}
//# sourceMappingURL=DataTreeModel.js.map