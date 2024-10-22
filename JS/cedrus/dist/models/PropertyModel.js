import { isDate, isNullOrUndefined, isString } from "@juniper-lib/util";
import { isDataType, isStorageType } from "./DataType";
import { isICreationTracked } from "./ICreationTracked";
import { isISequenced } from "./ISequenced";
import { isTypeStamped } from "./ITypeStamped";
import { TypedNameOrId } from "./NameOrId";
import { isPropertyTypeModel } from "./PropertyTypeModel";
import { isUserModel } from "./UserModel";
export function isPropertyModel(obj, dataOrStorageType, storageType) {
    const dataType = isDataType(dataOrStorageType)
        ? dataOrStorageType
        : null;
    storageType = isStorageType(dataOrStorageType)
        ? dataOrStorageType
        : storageType;
    return isTypeStamped("property", obj)
        && isISequenced(obj)
        && isICreationTracked(obj)
        && "type" in obj
        && isPropertyTypeModel(obj.type, dataType, storageType)
        && "value" in obj
        && "units" in obj
        && isString(obj.units)
        && "updatedBy" in obj
        && isUserModel(obj.updatedBy)
        && "updatedOn" in obj
        && (isNullOrUndefined(obj.updatedOn)
            || isDate(obj.updatedOn));
}
export function PROPERTY(idOrName) {
    return TypedNameOrId(idOrName, "property");
}
//# sourceMappingURL=PropertyModel.js.map