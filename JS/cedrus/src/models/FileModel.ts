import { isNumber, isString } from "@juniper-lib/util";
import { ICreationTracked, isICreationTracked } from "./ICreationTracked";
import { INamed, isINamed } from "./INamed";
import { isTypeStamped, ITypeStamped } from "./ITypeStamped";

export interface FileModel extends INamed, ITypeStamped<"file">, ICreationTracked {
    guid: string;
    type: string;
    size: number;
    formattedSize: string;
    path: string;
}

export function isFileModel(obj: unknown): obj is FileModel {
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