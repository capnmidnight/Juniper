import { isNumber, isString } from "@juniper-lib/util";
import { isICreationTracked } from "./ICreationTracked";
import { isINamed } from "./INamed";
import { isTypeStamped } from "./ITypeStamped";
export function isFileModel(obj) {
    return isINamed(obj)
        && isTypeStamped("file", obj)
        && isICreationTracked(obj)
        && "guid" in obj
        && isString(obj.guid)
        && "type" in obj
        && isString(obj.type)
        && "size" in obj
        && isNumber(obj.size)
        && "formattedSize" in obj
        && isString(obj.formattedSize)
        && "path" in obj
        && isString(obj.path);
}
//# sourceMappingURL=FileModel.js.map