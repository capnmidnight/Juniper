import { ICreationTracked } from "./ICreationTracked";
import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";

export interface FileModel extends INamed, ITypeStamped<"file">, ICreationTracked {
    guid: string;
    type: string;
    size: string;
    path: string;
}
