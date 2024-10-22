import { ICreationTracked } from "./ICreationTracked";
import { INamed } from "./INamed";
import { ITypeStamped } from "./ITypeStamped";
export interface FileModel extends INamed, ITypeStamped<"file">, ICreationTracked {
    guid: string;
    type: string;
    size: number;
    formattedSize: string;
    path: string;
}
export declare function isFileModel(obj: unknown): obj is FileModel;
//# sourceMappingURL=FileModel.d.ts.map